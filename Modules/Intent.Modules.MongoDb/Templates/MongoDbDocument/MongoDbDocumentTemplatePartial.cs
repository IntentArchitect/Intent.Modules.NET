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
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Modules.MongoDb.Templates.MongoDbDocumentInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using AttributeModel = Intent.Modelers.Domain.Api.AttributeModel;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbDocumentTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbDocumentTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MongoDB.Driver")
                .AddUsing("MongoDB.Bson.Serialization.Attributes")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Linq")
                .AddUsing("System")
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

                        @class.WithBaseType($"{this.GetMongoDbDocumentName(Model.ParentClass)}{genericTypeArguments}");
                    }

                    {
                        var genericTypeArguments = Model.GenericTypes.Any()
                            ? $"<{string.Join(", ", Model.GenericTypes)}>"
                            : string.Empty;
                        @class.ImplementsInterface($"{this.GetMongoDbDocumentInterfaceName()}{genericTypeArguments}");
                    }
                }).OnBuild(file =>
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
                        this.AddMongoDbDocumentProperties(
                            @class: @class,
                            attributes: attributes,
                            associationEnds: associationEnds,
                            documentInterfaceTemplateId: MongoDbDocumentInterfaceTemplate.TemplateId
                        );
                    }

                    var pk = Model.GetPrimaryKeyAttribute();
                    this.AddMongoDbMappingMethods(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: associationEnds,
                        entityInterfaceTypeName: EntityTypeName,
                        entityImplementationTypeName: EntityStateTypeName,
                        entityRequiresReflectionConstruction: Model.Constructors.Any() &&
                                                              Model.Constructors.All(x => x.Parameters.Count != 0),
                        entityRequiresReflectionPropertySetting: ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters(),
                        isAggregate: Model.IsAggregateRoot(),
                        hasBaseType: Model.ParentClass != null,
                        pk == null ? "string" : GetTypeName(pk),
                        pk == null ? "Id" : pk.Name
                    );
                });
        }

        private void AddPropertiesForAggregate(CSharpClass @class)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            var pk = Model.GetPrimaryKeyAttribute();

            if (pk == null)
            {
                return;
            }

            var genericTypeArguments = Model.GenericTypes.Any()
                ? $"<{string.Join(", ", Model.GenericTypes)}>"
                : string.Empty;
            var tDomainStateConstraint = createEntityInterfaces
                ? $", {EntityStateTypeName}{genericTypeArguments}"
                : string.Empty;
            @class.ImplementsInterface(
                $"{this.GetMongoDbDocumentOfTInterfaceName()}<{EntityTypeName}{genericTypeArguments}{tDomainStateConstraint}, {@class.Name}{genericTypeArguments}, {GetTypeName(pk)}>");


            var entityProperties = EntityStateFileBuilder.CSharpFile.Classes.First()
                .Properties.Where(x => x.ExplicitlyImplementing == null &&
                                       x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel)
                .ToArray();


            // If the PK is not derived and has a name other than "Id", then we need to do an explicit implementation for Id:
            //if (!string.Equals(pk.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
            //    entityProperties.Any(x => x.GetMetadata<IMetadataModel>("model").Id == pk.Id))
            //{
            //    var pkPropertyName = pk.Name.ToPascalCase();
            //    @class.AddProperty("string", "Id", property =>
            //    {
            //        property.AddAttribute($"{UseType("MongoDB.Bson.Serialization.Attributes.BsonId")}");
            //        // TODO Cater for GUIDS with a Setting
            //        property.Getter.WithExpressionImplementation($"{pkPropertyName}");
            //        property.Setter.WithExpressionImplementation($"{pkPropertyName} = value");
            //    });
            //}

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




                //// PK must always be a string
                //if (metadataModel.Id == pk.Id && !string.Equals(typeName, Helpers.PrimaryKeyType, StringComparison.OrdinalIgnoreCase))
                //{
                //    typeName = Helpers.PrimaryKeyType;
                //}

                if (metadataModel is AssociationEndModel)
                {
                    typeName = typeName.Replace(typeReference.Element.Name, $"I{typeReference.Element.Name}Document");
                }

                bool interfaceAccessorAdded = false;
                @class.AddProperty(typeName, entityProperty.Name, property =>
                {
                    if (metadataModel.Id == pk.Id && (string.Equals(typeName, Helpers.PrimaryKeyType, StringComparison.OrdinalIgnoreCase) || string.Equals(typeName, Helpers.PrimaryKeyTypeGuid, StringComparison.OrdinalIgnoreCase)))
                    {
                        property.AddAttribute("BsonId");
                        property.AddAttribute("BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)");
                    }
                    if (metadataModel is AttributeModel classAttribute1 && classAttribute1.HasFieldSettings())
                    {
                        property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"{classAttribute1.GetFieldSettings().Name()}\")");
                    }
                    else if (metadataModel is AssociationTargetEndModel targetEnd && targetEnd.HasFieldSettings())
                    {
                        property.AddAttribute($"{UseType("Newtonsoft.Json.JsonProperty")}(\"{targetEnd.GetFieldSettings().Name()}\")");
                    }
                    else
                    // Add "Id" property with JsonProperty attribute if not the PK
                    {
                        if (string.Equals(entityProperty.Name, "Id", StringComparison.OrdinalIgnoreCase) && metadataModel.Id != pk.Id)
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

                    if (classAttribute2.TypeReference?.Element?.SpecializationType == "Value Object")
                    {
                        AddDocumentInterfaceAccessor(@class, classAttribute2.TypeReference, entityProperty.Name);
                        interfaceAccessorAdded = true;
                    }
                });

                if (metadataModel is AssociationTargetEndModel targetEndModel)
                {
                    AddDocumentInterfaceAccessor(@class, targetEndModel.TypeReference, entityProperty.Name);
                    interfaceAccessorAdded = true;
                }

                //if (metadataModel is AttributeModel classAttribute3 && classAttribute3.TypeReference.IsCollection && !interfaceAccessorAdded)
                //{
                //    var nullablePostFix = "";
                //    var isNullable = classAttribute3.TypeReference.IsNullable;
                //    if (isNullable)
                //    {
                //        nullablePostFix = OutputTarget.GetProject().NullableEnabled ? "?" : "";
                //    }

                //    @class.AddProperty(
                //        type: $"{UseType("System.Collections.Generic.IEnumerable")}<{GetTypeName((IElement)classAttribute3.TypeReference.Element)}>{nullablePostFix}",
                //        name: entityProperty.Name,
                //        configure: property =>
                //        {
                //            property.ExplicitlyImplements(this.GetMongoDbDocumentInterfaceName());
                //            property.Getter.WithExpressionImplementation(entityProperty.Name);
                //            property.WithoutSetter();
                //        });
                //}
            }
        }

        private void AddDocumentInterfaceAccessor(CSharpClass @class, ITypeReference elementReference, string entityPropertyName)
        {
            //@class.AddProperty(this.GetDocumentInterfaceName(elementReference), entityPropertyName,
            //    property =>
            //    {
            //        property.ExplicitlyImplements(this.GetMongoDbDocumentInterfaceName());
            //        property.Getter.WithExpressionImplementation(entityPropertyName);
            //        property.WithoutSetter();
            //    });
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