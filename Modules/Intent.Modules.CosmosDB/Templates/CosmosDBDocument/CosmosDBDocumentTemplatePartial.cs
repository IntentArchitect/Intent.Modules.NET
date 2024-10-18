using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Intent.CosmosDB.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDB.Settings;
using Intent.Modules.CosmosDB.Templates.CosmosDBDocumentInterface;
using Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocument;
using Intent.Modules.Metadata.DocumentDB.Settings;
using Intent.Modules.Modelers.Domain.Settings;
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
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateId);
            AddTypeSource(CosmosDBValueObjectDocumentTemplate.TemplateId);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateRoles.Domain.ValueObject);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Document", @class =>
                {
                    @class.Internal();

                    if (Model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    if (Model.ParentClass != null)
                    {
                        var genericTypeArguments = Model.ParentClass.GenericTypes.Any()
                            ? $"<{string.Join(", ", Model.ParentClassTypeReference.GenericTypeParameters.Select(GetTypeName))}>"
                            : string.Empty;

                        @class.WithBaseType($"{this.GetCosmosDBDocumentName(Model.ParentClass)}{genericTypeArguments}");
                    }

                    {
                        var genericTypeArguments = Model.GenericTypes.Any()
                            ? $"<{string.Join(", ", Model.GenericTypes)}>"
                            : string.Empty;
                        @class.ImplementsInterface($"{this.GetCosmosDBDocumentInterfaceName()}{genericTypeArguments}");
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

                    if (Model.IsAggregateRoot())
                    {
                        AddPropertiesForAggregate(@class);
                    }
                    else
                    {
                        this.AddCosmosDBDocumentProperties(
                            @class: @class,
                            attributes: attributes,
                            associationEnds: associationEnds,
                            documentInterfaceTemplateId: CosmosDBDocumentInterfaceTemplate.TemplateId
                        );
                    }

                    var pk = Model.GetPrimaryKeyAttribute();
                    Model.TryGetPartitionAttribute(out var partitionAttribute);
                    this.AddCosmosDBMappingMethods(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: associationEnds,
                        partitionKeyAttribute: partitionAttribute,
                        entityInterfaceTypeName: EntityTypeName,
                        entityImplementationTypeName: EntityStateTypeName,
                        entityRequiresReflectionConstruction: Model.Constructors.Any() &&
                                                              Model.Constructors.All(x => x.Parameters.Count != 0),
                        entityRequiresReflectionPropertySetting: ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters(),
                        isAggregate: Model.IsAggregateRoot(),
                        hasBaseType: Model.ParentClass != null
                    );
                }, 1000);
        }

        private void AddPropertiesForAggregate(CSharpClass @class)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();

            var genericTypeArguments = Model.GenericTypes.Any()
                ? $"<{string.Join(", ", Model.GenericTypes)}>"
                : string.Empty;
            var tDomainStateConstraint = createEntityInterfaces
                ? $", {EntityStateTypeName}{genericTypeArguments}"
                : string.Empty;
            @class.ImplementsInterface(
                $"{this.GetCosmosDBDocumentOfTInterfaceName()}<{EntityTypeName}{genericTypeArguments}{tDomainStateConstraint}, {@class.Name}{genericTypeArguments}>");

            var pk = Model.GetPrimaryKeyAttribute();
            Model.TryGetPartitionAttribute(out var partitionAttribute);
            var entityProperties = EntityStateFileBuilder.CSharpFile.Classes.First()
                .Properties.Where(x => x.ExplicitlyImplementing == null &&
                                       x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel)
                .ToArray();

            // If the PK is not derived and has a name other than "Id", then we need to do an explicit implementation for IItem.Id:
            if (!string.Equals(pk.IdAttribute.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
                entityProperties.Any(x => x.GetMetadata<IMetadataModel>("model").Id == pk.IdAttribute.Id))
            {
                var pkPropertyName = pk.IdAttribute.Name.ToPascalCase();
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

                    this.GetCosmosDBDocumentTypeExtensionMethodsName(); // Ensure using is added for extension method
                    property.Getter.WithExpressionImplementation("_type ??= GetType().GetNameForDocument()");
                    property.Setter.WithExpressionImplementation("_type = value");
                });
            }

            // ICosmosDBDocument.PartitionKey implementation:
            {
                if (partitionAttribute != null)
                {
                    @class.AddProperty("string?", "PartitionKey", property =>
                    {
                        property.ExplicitlyImplements(this.GetCosmosDBDocumentOfTInterfaceName());
                        property.Getter.WithExpressionImplementation($"{partitionAttribute.Name.ToPascalCase()}");
                        property.Setter.WithExpressionImplementation($"{partitionAttribute.Name.ToPascalCase()} = value!");
                    });
                }
            }


            var useOptimisticConcurrency = ExecutionContext.Settings.GetCosmosDb().UseOptimisticConcurrency() && Model.ParentClass == null;
            if (useOptimisticConcurrency)
            {
                // Etag implementation:
                @class.AddField("string?", "_etag", field =>
                {
                    field.Protected();
                    field.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"_etag\")");
                });

                @class.AddProperty("string?", "Etag", property =>
                {
                    property.ExplicitlyImplements("IItemWithEtag");
                    property.Getter.WithExpressionImplementation("_etag");
                    property.WithoutSetter();
                });
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
                if (metadataModel.Id == pk.IdAttribute.Id && !string.Equals(typeName, Helpers.PrimaryKeyType, StringComparison.OrdinalIgnoreCase))
                {
                    typeName = Helpers.PrimaryKeyType;
                }

                //Partition key must be a string
                if (partitionAttribute != null && metadataModel.Id == partitionAttribute.Id && !string.Equals(typeName, Helpers.PrimaryKeyType, StringComparison.OrdinalIgnoreCase))
                {
                    typeName = "string";
                }

                if (entityProperty.Name.ToLower() == "etag" && useOptimisticConcurrency && entityProperty.Type != "string?")
                {
                    Logging.Log.Failure("ETag attribute must be modeled as a nullable string.");
                }

                @class.AddProperty(typeName, entityProperty.Name, property =>
                {
                    if (entityProperty.Name.ToLower() == "etag" && useOptimisticConcurrency)
                    {
                        property.Getter.WithExpressionImplementation("_etag");
                        property.Setter.WithExpressionImplementation("_etag = value");
                        property.AddAttribute("JsonIgnore");
                    }
                    else if (IsNonNullableReferenceType(typeReference))
                    {
                        property.WithInitialValue("default!");
                    }

                    if (metadataModel is AttributeModel classAttribute1 && classAttribute1.HasFieldSetting())
                    {
                        property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"{classAttribute1.GetFieldSetting().Name()}\")");
                    }
                    else if (metadataModel is AssociationTargetEndModel targetEnd && targetEnd.HasFieldSetting())
                    {
                        property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"{targetEnd.GetFieldSetting().Name()}\")");
                    }
                    else
                    // Add "Id" property with JsonProperty attribute if not the PK
                    {
                        if (string.Equals(entityProperty.Name, "Id", StringComparison.OrdinalIgnoreCase) && metadataModel.Id != pk.IdAttribute.Id)
                        {
                            property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"@id\")");
                        }

                        // Add "Type" property with JsonProperty attribute so as to not conflict with the IItem.Type property
                        else if (string.Equals(entityProperty.Name, "Type", StringComparison.OrdinalIgnoreCase))
                        {
                            property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"@type\")");
                        }
                    }

                    if (metadataModel is not AttributeModel classAttribute2)
                    {
                        return;
                    }

                    if (classAttribute2.TypeReference?.Element?.IsEnumModel() == true &&
                        ExecutionContext.Settings.GetCosmosDb().StoreEnumsAsStrings())
                    {
                        property.AddAttribute($"{UseType("Newtonsoft.Json.JsonConverter")}(typeof({this.GetEnumJsonConverterName()}))");
                    }

                    if (classAttribute2.TypeReference?.Element?.SpecializationType == "Value Object")
                    {
                        AddDocumentInterfaceAccessor(@class, classAttribute2.TypeReference, entityProperty.Name);
                    }
                });

                if (metadataModel is AssociationTargetEndModel targetEndModel)
                {
                    AddDocumentInterfaceAccessor(@class, targetEndModel.TypeReference, entityProperty.Name);
                }

                if (metadataModel is AttributeModel classAttribute3 && classAttribute3.TypeReference.IsCollection)
                {
                    var nullablePostFix = "";
                    var isNullable = classAttribute3.TypeReference.IsNullable;
                    if (isNullable)
                    {
                        nullablePostFix = OutputTarget.GetProject().NullableEnabled ? "?" : "";
                    }

                    @class.AddProperty(
                        type: $"{UseType("System.Collections.Generic.IReadOnlyList")}<{GetTypeName((IElement)classAttribute3.TypeReference.Element)}>{nullablePostFix}",
                        name: entityProperty.Name,
                        configure: property =>
                        {
                            property.ExplicitlyImplements(this.GetCosmosDBDocumentInterfaceName());
                            property.Getter.WithExpressionImplementation(entityProperty.Name);
                            property.WithoutSetter();
                        });
                }
            }
        }

        private void AddDocumentInterfaceAccessor(CSharpClass @class, ITypeReference elementReference, string entityPropertyName)
        {
            @class.AddProperty(this.GetDocumentInterfaceName(elementReference), entityPropertyName,
                property =>
                {
                    property.ExplicitlyImplements(this.GetCosmosDBDocumentInterfaceName());
                    property.Getter.WithExpressionImplementation(entityPropertyName);
                    property.WithoutSetter();
                });
        }

        public string EntityStateTypeName => GetTypeName(TemplateRoles.Domain.Entity.Primary, Model);
        public string EntityTypeName => GetTypeName(TemplateRoles.Domain.Entity.Interface, Model);

        public ICSharpFileBuilderTemplate EntityStateFileBuilder => GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model);

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