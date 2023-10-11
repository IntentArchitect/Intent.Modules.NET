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
using Intent.Modules.Constants;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageTableEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TableStorageTableEntityTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.TableStorageTableEntity";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TableStorageTableEntityTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            AddTypeSource(TemplateId);
            AddTypeSource(TemplateFulfillingRoles.Domain.Enum);
            AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}TableEntity", @class =>
                {
                    @class.Internal();

                    if (model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    foreach (var genericType in model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    if (model.ParentClass != null)
                    {
                        var genericTypeArguments = model.ParentClass.GenericTypes.Any()
                            ? $"<{string.Join(", ", model.ParentClassTypeReference.GenericTypeParameters.Select(GetTypeName))}>"
                            : string.Empty;
                        @class.WithBaseType($"{this.GetTableStorageTableEntityName(model.ParentClass)}{genericTypeArguments}");
                    }
                })
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var entityPropertyIds = EntityStateFileBuilder.CSharpFile.Classes.First().Properties
                        .Select(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel
                                ? metadataModel.Id
                                : null)
                        .Where(x => x != null)
                        .ToHashSet();

                    var attributes = Model.Attributes
                        .Where(x => entityPropertyIds.Contains(x.Id))
                        .ToList();
                    var associationEnds = Model.AssociatedClasses
                        .Where(x => entityPropertyIds.Contains(x.Id) && x.IsNavigable)
                        .ToList();

                    if (model.IsAggregateRoot())
                    {
                        /*
                        AddPropertiesForAggregate(@class);
                        this.AddCosmosDBMappingMethods(
                            template: this,
                            @class: @class,
                            attributes: attributes,
                            associationEnds: associationEnds,
                            entityInterfaceTypeName: EntityInterfaceName,
                            entityStateTypeName: EntityStateName,
                            entityRequiresReflectionConstruction: Model.Constructors.Any() &&
                                                                  Model.Constructors.All(x => x.Parameters.Count != 0),
                            entityRequiresReflectionPropertySetting: ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters(),
                            isAggregate: model.IsAggregateRoot(),
                            hasBaseType: model.ParentClass != null);
                        */
                    }

                }, 1000);

        }

        /*
        public void AddCosmosDBMappingMethods(
            ICSharpTemplate template,
            CSharpClass @class,
            IReadOnlyList<AttributeModel> attributes,
            IReadOnlyList<AssociationEndModel> associationEnds,
            string entityInterfaceTypeName,
            string entityStateTypeName,
            bool entityRequiresReflectionConstruction,
            bool entityRequiresReflectionPropertySetting,
            bool isAggregate,
            bool hasBaseType)
        {
            var genericTypeArguments = @class.GenericParameters.Any()
                ? $"<{string.Join(", ", @class.GenericParameters.Select(x => x.TypeName))}>"
                : string.Empty;

            @class.AddMethod($"{entityInterfaceTypeName}{genericTypeArguments}", "ToEntity", method =>
            {
                method.AddParameter($"{entityStateTypeName}{genericTypeArguments}?", "entity", p =>
                {
                    if (!@class.IsAbstract)
                    {
                        p.WithDefaultValue("default");
                    }
                });

                if (!@class.IsAbstract)
                {
                    var instantiation = entityRequiresReflectionConstruction
                        ? $"{template.GetReflectionHelperName()}.CreateNewInstanceOf<{entityStateTypeName}{genericTypeArguments}>()"
                        : $"new {entityStateTypeName}{genericTypeArguments}()";

                    method.AddStatement($"entity ??= {instantiation};");
                }
                else
                {
                    method.AddIfStatement("entity is null", @if =>
                    {
                        @if.AddStatement($"throw new {template.UseType("System.ArgumentNullException")}(nameof(entity));");
                    });
                }

                for (var index = 0; index < attributes.Count; index++)
                {
                    var attribute = attributes[index];
                    var assignmentValueExpression = attribute.Name;

                    // If this is the PK which is not a string, we need to convert it:
                    var attributeTypeName = attribute.TypeReference.Element?.Name.ToLowerInvariant();
                    if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    {
                        assignmentValueExpression = attributeTypeName switch
                        {
                            "int" or "long" => $"{attributeTypeName}.Parse({assignmentValueExpression}, {template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => $"{template.UseType("System.Guid")}.Parse({assignmentValueExpression})",
                            _ => throw new Exception(
                                $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " + $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
                    }

                    method.AddStatement(
                        entityRequiresReflectionPropertySetting
                            ? $"{template.GetReflectionHelperName()}.ForceSetProperty(entity, nameof({attribute.Name.ToPascalCase()}), {assignmentValueExpression});"
                            : $"entity.{attribute.Name.ToPascalCase()} = {assignmentValueExpression};",
                        index == 0 ? s => s.SeparatedFromPrevious() : null);
                }

                foreach (var associationEnd in associationEnds)
                {
                    var nullable = associationEnd.IsNullable ? "?" : string.Empty;
                    var nullableSuppression = associationEnd.IsNullable || entityRequiresReflectionPropertySetting ? string.Empty : "!";

                    var assignmentValueExpression = $"{associationEnd.Name}{nullable}.ToEntity(){nullableSuppression}";

                    if (associationEnd.IsCollection)
                    {
                        assignmentValueExpression = $"{associationEnd.Name}{nullable}.Select(x => x.ToEntity()).ToList()";
                    }

                    method.AddStatement(entityRequiresReflectionPropertySetting
                        ? $"{template.GetReflectionHelperName()}.ForceSetProperty(entity, nameof({associationEnd.Name.ToPascalCase()}), {assignmentValueExpression});"
                        : $"entity.{associationEnd.Name.ToPascalCase()} = {assignmentValueExpression};");
                }

                if (hasBaseType)
                {
                    method.AddStatement("base.ToEntity(entity);");
                }

                method.AddStatement("return entity;", s => s.SeparatedFromPrevious());
            });

            @class.AddMethod($"{@class.Name}{genericTypeArguments}", "PopulateFromEntity", method =>
            {
                method.AddParameter($"{entityInterfaceTypeName}{genericTypeArguments}", "entity");

                foreach (var attribute in attributes)
                {
                    var suffix = string.Empty;

                    // If this is the PK which is not a string, we need to convert it:
                    var attributeTypeName = attribute.TypeReference.Element?.Name.ToLowerInvariant();
                    if (isAggregate && attribute.HasPrimaryKey() && !string.Equals(attributeTypeName, "string"))
                    {
                        suffix = attributeTypeName switch
                        {
                            "int" or "long" => $".ToString({template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                            "guid" => ".ToString()",
                            _ => throw new Exception(
                                $"Unsupported primary key type \"{attributeTypeName}\" [{attribute.TypeReference.Element?.Id}] for attribute " + $"\"{attribute.Name}\" [{attribute.Id}] " +
                                $"on \"{attribute.InternalElement.ParentElement.Name}\" [{attribute.InternalElement.ParentElement.Id}]")
                        };
                    }

                    method.AddStatement($"{attribute.Name} = entity.{attribute.Name}{suffix};");
                }

                foreach (var associationEnd in associationEnds)
                {
                    var documentTypeName = template.GetTypeName((IElement)associationEnd.TypeReference.Element);

                    if (associationEnd.IsCollection)
                    {
                        template.AddUsing("System.Linq");

                        var nullable = associationEnd.IsNullable ? "?" : string.Empty;
                        method.AddStatement($"{associationEnd.Name} = entity.{associationEnd.Name}{nullable}.Select(x => {documentTypeName}.FromEntity(x)!).ToList();");
                        continue;
                    }

                    var nullableSuppression = associationEnd.IsNullable ? string.Empty : "!";
                    method.AddStatement($"{associationEnd.Name} = {documentTypeName}.FromEntity(entity.{associationEnd.Name}){nullableSuppression};");
                }

                if (hasBaseType)
                {
                    method.AddStatement("base.PopulateFromEntity(entity);");
                }

                method.AddStatement("return this;", s => s.SeparatedFromPrevious());
            });

            if (!@class.IsAbstract)
            {
                @class.AddMethod($"{@class.Name}{genericTypeArguments}?", "FromEntity", method =>
                {
                    method.AddParameter($"{entityInterfaceTypeName}{genericTypeArguments}?", "entity");
                    method.Static();

                    method.AddIfStatement("entity is null", @if => @if.AddStatement("return null;"));
                    method.AddStatement($"return new {@class.Name}{genericTypeArguments}().PopulateFromEntity(entity);", s => s.SeparatedFromPrevious());
                });
            }
        }

        private void AddPropertiesForAggregate(CSharpClass @class)
        {
            var genericTypeArguments = Model.GenericTypes.Any()
                ? $"<{string.Join(", ", Model.GenericTypes)}>"
                : string.Empty;
            @class.ImplementsInterface(
                $"{this.GetTableStorageTableIEntityInterfaceName()}<{EntityInterfaceName}{genericTypeArguments}, {@class.Name}{genericTypeArguments}>");

            var pkAttribute = Model.GetPrimaryKeyAttribute();
            var entityProperties = EntityStateFileBuilder.CSharpFile.Classes.First()
                .Properties.Where(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel)
                .ToArray();

            // If the PK is not derived and has a name other than "Id", then we need to do an explicit implementation for IItem.Id:
            if (!string.Equals(pkAttribute.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
                entityProperties.Any(x => x.GetMetadata<IMetadataModel>("model").Id == pkAttribute.Id))
            {
                var pkPropertyName = pkAttribute.Name.ToPascalCase();
                @class.AddProperty("string", "Id", property =>
                {
                    property.ExplicitlyImplements(UseType("Microsoft.Azure.CosmosRepository.IItem"));
                    property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"id\")");
                    property.Getter.WithExpressionImplementation($"{pkPropertyName}");
                    property.Setter.WithExpressionImplementation($"{pkPropertyName} = value");
                });
            }

            // IItem.Type implementation:
            if (Model.ParentClass == null)
            {
                @class.AddField("string?", "_type");
                @class.AddProperty("string", "Type", property =>
                {
                    property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"type\")")
                        .ExplicitlyImplements(UseType("Microsoft.Azure.CosmosRepository.IItem"));

                    //this.GetCosmosDBDocumentTypeExtensionMethodsName(); // Ensure using is added for extension method
                    property.Getter.WithExpressionImplementation("_type ??= GetType().GetNameForDocument()");
                    property.Setter.WithExpressionImplementation("_type = value");
                });
            }
            
            // ICosmosDBDocument.PartitionKey implementation:
            {
                Model.TryGetContainerSettings(out var containerName, out var partitionKey);
                var partitionKeyAttribute = partitionKey == null
                    ? pkAttribute
                    : Model.GetAttributeOrDerivedWithName(partitionKey);
                if (partitionKeyAttribute != null && partitionKeyAttribute.Id != pkAttribute.Id)
                {
                    @class.AddProperty("string?", "PartitionKey", property =>
                    {
                        property.ExplicitlyImplements(this.GetCosmosDBDocumentInterfaceName());
                        property.Getter.WithExpressionImplementation($"{partitionKeyAttribute.Name.ToPascalCase()}{partitionKeyAttribute.GetToString(this)}");
                        property.Setter.WithExpressionImplementation($"{partitionKeyAttribute.Name.ToPascalCase()}{partitionKeyAttribute.GetToString(this)} = value!");
                    });
                }
                else if (partitionKeyAttribute == null)
                {
                    Logging.Log.Warning($"Class \"{Model.Name}\" [{Model.Id}] does not have an attribute with name matching " + $"partition key " +
                                        $"\"{partitionKey}\" for \"{containerName}\" container.");
                }
            }

            foreach (var entityProperty in entityProperties)
            {
                var metadataModel = entityProperty.GetMetadata<IMetadataModel>("model");
                var typeReference = metadataModel switch
                {
                    AttributeModel attribute => attribute.TypeReference,
                    AssociationEndModel associationEnd => associationEnd,
                    _ => throw new InvalidOperationException()
                };

                var typeName = GetTypeName(typeReference);

                // PK must always be a string
                if (metadataModel.Id == pkAttribute.Id && !string.Equals(typeName, "string", StringComparison.OrdinalIgnoreCase))
                {
                    typeName = "string";
                }

                @class.AddProperty(typeName, entityProperty.Name, property =>
                {
                    if (IsNonNullableReferenceType(typeReference))
                    {
                        property.WithInitialValue("default!");
                    }

                    if (string.Equals(entityProperty.Name, "Id", StringComparison.OrdinalIgnoreCase) && metadataModel.Id != pkAttribute.Id)
                    {
                        property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"@id\")");
                    }

                    // Add "Type" property with JsonProperty attribute so as to not conflict with the IItem.Type property
                    else if (string.Equals(entityProperty.Name, "Type", StringComparison.OrdinalIgnoreCase))
                    {
                        property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"@type\")");
                    }
                });
            }
        }*/

        public string EntityInterfaceName => GetTypeName(TemplateFulfillingRoles.Domain.Entity.Interface, Model);

        public string EntityStateName => GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, Model);

        public ICSharpFileBuilderTemplate EntityStateFileBuilder => GetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, Model);


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