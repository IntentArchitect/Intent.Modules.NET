using System;
using System.Collections.Generic;
using System.Linq;
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
        public const string TemplateId = "Intent.Modules.CosmosDB.CosmosDBDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBDocumentTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            AddTypeSource(TemplateId);
            AddTypeSource("Intent.Entities.DomainEnum");
            AddTypeSource("Domain.Entity");
            AddTypeSource("Domain.ValueObject");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Document", @class =>
                {
                    var itemInterfaceTypeName = UseType("Microsoft.Azure.CosmosRepository.IItem");
                    
                    Model.TryGetContainerSettings(out var containerName, out var partitionKey);

                    var idAttribute = Model.Attributes.Single(x => x.HasPrimaryKey());
                    var idPropertyName = idAttribute.Name.ToPascalCase();
                    var partitionKeyAttribute = partitionKey == null
                        ? idAttribute
                        : model.Attributes.SingleOrDefault(x => partitionKey.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

                    @class.Internal();

                    if (model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    if (Model.ParentClass != null)
                    {
                        @class.ExtendsClass(GetTemplate<ICSharpFileBuilderTemplate>(TemplateId, Model.ParentClass.Id).CSharpFile.Classes.First());
                    }

                    @class.ImplementsInterface(itemInterfaceTypeName);

                    @class.AddField("string?", "_type");

                    var properties = new List<(string Name, ITypeReference TypeReference, IElement Element)>();
                    foreach (var attribute in Model.Attributes)
                    {
                        properties.Add((attribute.Name, attribute.TypeReference, attribute.InternalElement));
                    }

                    foreach (var targetEnd in Model.AssociatedToClasses())
                    {
                        if (targetEnd.Element.AsClassModel()?.IsAggregateRoot() == true)
                        {
                            continue;
                        }

                        properties.Add((targetEnd.Name, targetEnd.TypeReference, targetEnd.InternalAssociationEnd));
                    }

                    var nonNullableReferenceTypePropertyNames = properties
                        .Where(x => IsNonNullableReferenceType(x.TypeReference))
                        .Select(x => x.Name.ToPascalCase())
                        .ToArray();
                    if (nonNullableReferenceTypePropertyNames.Any())
                    {
                        @class.AddConstructor(constructor =>
                        {
                            foreach (var name in nonNullableReferenceTypePropertyNames)
                            {
                                constructor.AddStatement($"{name.ToPascalCase()} = null!;");
                            }
                        });
                    }

                    if (idPropertyName is not "Id")
                    {
                        @class.AddProperty("string", "Id", property =>
                        {
                            property
                                .AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"id\")")
                                .ExplicitlyImplements(itemInterfaceTypeName);

                            property.Getter.WithExpressionImplementation(idPropertyName);
                            property.Setter.WithExpressionImplementation($"{idPropertyName} = value");
                        });
                    }

                    @class.AddProperty("string", "Type", property =>
                    {
                        property
                            .AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"type\")")
                            .ExplicitlyImplements(itemInterfaceTypeName);

                        property.Getter.WithExpressionImplementation("_type ??= GetType().Name");
                        property.Setter.WithExpressionImplementation("_type = value");
                    });



                    if (partitionKeyAttribute != null)
                    {
                        @class.AddProperty("string", "PartitionKey", p => p
                            .ExplicitlyImplements(itemInterfaceTypeName)
                            .WithoutSetter()
                            .Getter.WithExpressionImplementation(partitionKeyAttribute.Name.ToPascalCase())
                        );
                    }
                    else
                    {
                        Logging.Log.Warning($"Class \"{Model.Name}\" [{Model.Id}] does not have attribute with name matching " +
                                            $"partition key \"{partitionKey}\" for \"{containerName}\" container.");
                    }

                    foreach (var (name, typeReference, element) in properties)
                    {
                        @class.AddProperty(GetTypeName(typeReference), name.ToPascalCase(), property =>
                        {
                            property.TryAddXmlDocComments(element);

                            if ("id".Equals(name, StringComparison.OrdinalIgnoreCase))
                            {
                                var fieldName = element == idAttribute.InternalElement
                                    ? "id"
                                    : "@id";

                                property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"{fieldName}\")");
                            }

                            if ("type".Equals(name, StringComparison.OrdinalIgnoreCase))
                            {
                                property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"@type\")");
                            }
                        });
                    }
                });
        }

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