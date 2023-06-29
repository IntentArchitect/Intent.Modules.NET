using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBDocumentTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBDocumentTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            AddTypeSource(TemplateId);
            AddTypeSource(TemplateFulfillingRoles.Domain.Enum);
            AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Document", @class =>
                {
                    @class.Internal();

                    if (model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    @class
                        .ExtendsClass(EntityStateName)
                        .ImplementsInterface($"{this.GetCosmosDBDocumentInterfaceName()}<{@class.Name}, {EntityInterfaceName}>");

                    @class.AddField("string?", "_type");

                    // IItem.Id implementation:
                    var pkAttribute = Model.Attributes.SingleOrDefault(x => x.HasPrimaryKey());
                    if (pkAttribute != null)
                    {
                        var pkPropertyName = pkAttribute.Name.ToPascalCase();
                        var pkTypeName = pkAttribute.TypeReference.Element?.Name.ToLowerInvariant();
                        @class.AddProperty("string", "Id", property =>
                        {
                            property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"id\")");
                            string baseQualifier;
                            if (pkPropertyName == "Id")
                            {
                                baseQualifier = "base.";
                                property.New();
                            }
                            else
                            {
                                baseQualifier = string.Empty;
                                property.ExplicitlyImplements(UseType("Microsoft.Azure.CosmosRepository.IItem"));
                            }

                            switch (pkTypeName)
                            {
                                case "int" or "long":
                                    {
                                        var invariantCulture = $"{UseType("System.Globalization.CultureInfo")}.InvariantCulture";
                                        property.Getter.WithExpressionImplementation($"{baseQualifier}{pkPropertyName}.ToString({invariantCulture})");
                                        property.Setter.WithExpressionImplementation($"{baseQualifier}{pkPropertyName} = {pkTypeName}.Parse(value, {invariantCulture})");
                                        break;
                                    }
                                case "string":
                                    {
                                        property.Getter.WithExpressionImplementation($"{baseQualifier}{pkPropertyName} ??= {UseType("System.Guid")}.NewGuid().ToString()");
                                        property.Setter.WithExpressionImplementation($"{baseQualifier}{pkPropertyName} = value");
                                        break;
                                    }
                                case "guid":
                                    {
                                        property.Getter.WithExpressionImplementation($"{baseQualifier}{pkPropertyName}.ToString()");
                                        property.Setter.WithExpressionImplementation($"{baseQualifier}{pkPropertyName} = {UseType("System.Guid")}.Parse(value)");
                                        break;
                                    }
                                default:
                                    throw new Exception(
                                        $"Unsupported primary key type \"{pkTypeName}\" [{pkAttribute.TypeReference.Element?.Id}] for attribute " +
                                        $"\"{pkAttribute.Name}\" [{pkAttribute.Id}] on Class \"{Model.Name}\" [{Model.Id}]");
                            }
                        });
                    }

                    // IItem.Type implementation:
                    @class.AddProperty("string", "Type", property =>
                    {
                        property
                            .AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"type\")")
                            .ExplicitlyImplements(UseType("Microsoft.Azure.CosmosRepository.IItem"));

                        property.Getter.WithExpressionImplementation("_type ??= GetType().Name");
                        property.Setter.WithExpressionImplementation("_type = value");
                    });

                    // IItem.PartitionKey implementation:
                    Model.TryGetContainerSettings(out var containerName, out var partitionKey);
                    var partitionKeyAttribute = partitionKey == null
                        ? pkAttribute
                        : model.Attributes.SingleOrDefault(x => partitionKey.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                    if (partitionKeyAttribute != null &&
                        partitionKeyAttribute.Id != pkAttribute?.Id)
                    {
                        @class.AddProperty("string", "PartitionKey", p => p
                            .ExplicitlyImplements(UseType("Microsoft.Azure.CosmosRepository.IItem"))
                            .WithoutSetter()
                            .Getter.WithExpressionImplementation($"{partitionKeyAttribute.Name.ToPascalCase()}{partitionKeyAttribute.GetToString(this)}")
                        );
                    }
                    else if (partitionKeyAttribute == null)
                    {
                        Logging.Log.Warning($"Class \"{Model.Name}\" [{Model.Id}] does not have attribute with name matching " +
                                            $"partition key \"{partitionKey}\" for \"{containerName}\" container.");
                    }

                    // Add "Id" property with JsonProperty attribute if not the PK
                    var idAttribute = Model.Attributes.FirstOrDefault(x => "id".Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                    if (idAttribute?.HasPrimaryKey() == false)
                    {
                        @class.AddProperty(GetTypeName(idAttribute), "Id", property =>
                        {
                            property.New();
                            property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"@id\")");
                            property.Getter.WithExpressionImplementation("base.Id");
                            property.Setter.WithExpressionImplementation("base.Id = value");
                        });
                    }

                    // Add "Type" property with JsonProperty attribute so as to not conflict with the IItem.Type property
                    var typeAttribute = Model.Attributes.FirstOrDefault(x => "type".Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                    if (typeAttribute != null)
                    {
                        @class.AddProperty(GetTypeName(typeAttribute), "Type", property =>
                        {
                            property.New();
                            property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"@type\")");
                            property.Getter.WithExpressionImplementation("base.Type");
                            property.Setter.WithExpressionImplementation("base.Type = value");
                        });
                    }

                    @class.AddMethod(@class.Name, "PopulateFromEntity", method =>
                    {
                        method.AddParameter(EntityInterfaceName, "entity");

                        var currentClass = Model;
                        var alreadyHandledProperties = new HashSet<string>();

                        while (currentClass != null)
                        {
                            foreach (var attribute in currentClass.Attributes)
                            {
                                var name = attribute.Name.ToPascalCase();
                                if (!alreadyHandledProperties.Add(name))
                                {
                                    continue;
                                }

                                var toString = name == "Id" &&
                                               attribute.HasPrimaryKey() &&
                                               attribute.TypeReference.Element?.Name != "string"
                                    ? attribute.GetToString(this)
                                    : string.Empty;

                                method.AddStatement($"{name} = entity.{name}{toString};");
                            }

                            foreach (var association in currentClass.AssociatedToClasses())
                            {
                                var name = association.Name.ToPascalCase();
                                if (association.Element.AsClassModel()?.IsAggregateRoot() == true ||
                                    !alreadyHandledProperties.Add(name))
                                {
                                    continue;
                                }

                                method.AddStatement($"{name} = entity.{name};");
                            }

                            currentClass = currentClass.ParentClass;
                        }

                        method.AddStatement("return this;", s => s.SeparatedFromPrevious());
                    });
                });
        }

        public string EntityInterfaceName => GetTypeName(TemplateFulfillingRoles.Domain.Entity.Interface, Model);

        public string EntityStateName => GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, Model);

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}