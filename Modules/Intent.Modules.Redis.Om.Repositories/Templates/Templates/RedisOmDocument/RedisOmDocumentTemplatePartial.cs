using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocumentInterface;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmValueObjectDocument;
using Intent.Redis.Om.Repositories.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmDocumentTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmDocumentTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateId);
            AddTypeSource(RedisOmValueObjectDocumentTemplate.TemplateId);
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

                        @class.WithBaseType($"{this.GetRedisOmDocumentName(Model.ParentClass)}{genericTypeArguments}");
                    }

                    {
                        var genericTypeArguments = Model.GenericTypes.Any()
                            ? $"<{string.Join(", ", Model.GenericTypes)}>"
                            : string.Empty;
                        @class.ImplementsInterface($"{this.GetRedisOmDocumentInterfaceName()}{genericTypeArguments}");
                    }

                    if (Model.IsAggregateRoot())
                    {
                        @class.AddAttribute(UseType("Redis.OM.Modeling.Document"), attr => attr
                            .AddArgument("StorageType = StorageType.Json")
                            .AddArgument($@"Prefixes = new []{{ ""{Model.Name}"" }}"));
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
                        this.AddRedisOmDocumentProperties(
                            @class: @class,
                            attributes: attributes,
                            associationEnds: associationEnds,
                            documentInterfaceTemplateId: RedisOmDocumentInterfaceTemplate.TemplateId);
                    }

                    Model.TryGetPartitionAttribute(out var partitionAttribute);
                    this.AddRedisOmMappingMethods(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: associationEnds,
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
            ImplementInterfaceForClass(@class);
            AddPropertiesFromEntityState(@class);
        }

        private void ImplementInterfaceForClass(CSharpClass @class)
        {
            var interfaceName = this.GetRedisOmDocumentOfTInterfaceName();
            var entityTypeArguments = Model.GenericTypes.Any() ? $"<{string.Join(", ", Model.GenericTypes)}>" : string.Empty;
            var domainStateConstraint = GetDomainStateConstraint();

            @class.ImplementsInterface($"{interfaceName}<{EntityTypeName}{entityTypeArguments}{domainStateConstraint}, {@class.Name}{entityTypeArguments}>");

            return;
            string GetGenericTypeArguments()
            {
                return Model.GenericTypes.Any() ? $"<{string.Join(", ", Model.GenericTypes)}>" : string.Empty;
            }

            string GetDomainStateConstraint()
            {
                var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
                return createEntityInterfaces ? $", {EntityStateTypeName}{GetGenericTypeArguments()}" : string.Empty;
            }
        }

        private void AddPropertiesFromEntityState(CSharpClass @class)
        {
            var entityProperties = EntityStateFileBuilder.CSharpFile.Classes.First()
                .Properties.Where(IsValidPropertyForAdding)
                .ToArray();

            foreach (var entityProperty in entityProperties)
            {
                AddPropertyToClass(@class, entityProperty);
            }
        }

        private bool IsValidPropertyForAdding(CSharpProperty property)
        {
            return property.ExplicitlyImplementing == null && property.TryGetMetadata<IMetadataModel>("model", out var metadataModel) &&
                   metadataModel is AttributeModel or AssociationEndModel;
        }

        private void AddPropertyToClass(CSharpClass @class, CSharpProperty entityProperty)
        {
            var entityPropertyMetadata = entityProperty.GetMetadata<IMetadataModel>("model");
            var entityTargetModel = entityPropertyMetadata switch
            {
                AttributeModel attribute => attribute.TypeReference,
                AssociationEndModel associationEnd => associationEnd,
                _ => throw new InvalidOperationException()
            };
            var typeName = GetTypeNameBasedOnPrimaryKey(entityPropertyMetadata, entityTargetModel);

            @class.AddProperty(typeName, entityProperty.Name, documentProperty => ConfigureDocumentProperty(documentProperty, entityPropertyMetadata, entityTargetModel, entityProperty));

            if (entityPropertyMetadata is AssociationTargetEndModel targetEndModel)
            {
                AddAssociationProperty(@class, entityProperty, targetEndModel);
            }

            if (entityPropertyMetadata is AttributeModel attributeModel && attributeModel.TypeReference.IsCollection)
            {
                AddCollectionProperty(@class, entityProperty, attributeModel);
            }
        }

        private string GetTypeNameBasedOnPrimaryKey(IMetadataModel entityPropertyMetadata, ITypeReference entityTargetModel)
        {
            var typeName = GetTypeName(entityTargetModel);
            var isPrimaryKey = IsPrimaryKey(entityPropertyMetadata);

            if (isPrimaryKey && !string.Equals(typeName, Helpers.PrimaryKeyType, StringComparison.OrdinalIgnoreCase))
            {
                typeName = Helpers.PrimaryKeyType;
            }

            return typeName;
        }

        private void ConfigureDocumentProperty(CSharpProperty documentProperty, IMetadataModel entityPropertyMetadata, ITypeReference entityTargetModel, CSharpProperty entityProperty)
        {
            if (IsNonNullableReferenceType(entityTargetModel))
            {
                documentProperty.WithInitialValue("default!");
            }

            AddAttributesBasedOnMetadataModel(documentProperty, entityPropertyMetadata);
        }

        private void AddAssociationProperty(CSharpClass @class, CSharpProperty entityProperty, AssociationTargetEndModel entityPropertyMetadata)
        {
            var nullablePostFix = GetNullablePostfix(entityPropertyMetadata.TypeReference.IsNullable);
            @class.AddProperty($"{this.GetDocumentInterfaceName(entityPropertyMetadata.TypeReference)}{nullablePostFix}", entityProperty.Name,
                documentProperty => { ConfigureAssociationDocumentProperty(documentProperty, entityProperty); });
        }

        private void AddCollectionProperty(CSharpClass @class, CSharpProperty entityProperty, AttributeModel attributeModel)
        {
            var nullablePostFix = GetNullablePostfix(attributeModel.TypeReference.IsNullable);
            @class.AddProperty($"{UseType("System.Collections.Generic.IReadOnlyList")}<{GetTypeName((IElement)attributeModel.TypeReference.Element)}>{nullablePostFix}",
                entityProperty.Name, property => { ConfigureCollectionProperty(property, entityProperty); });
        }

        private void ConfigureAssociationDocumentProperty(CSharpProperty documentProperty, CSharpProperty entityProperty)
        {
            documentProperty.ExplicitlyImplements(this.GetRedisOmDocumentInterfaceName());
            documentProperty.Getter.WithExpressionImplementation(entityProperty.Name);
            documentProperty.WithoutSetter();
        }

        private void ConfigureCollectionProperty(CSharpProperty property, CSharpProperty entityProperty)
        {
            property.ExplicitlyImplements(this.GetRedisOmDocumentInterfaceName());
            property.Getter.WithExpressionImplementation(entityProperty.Name);
            property.WithoutSetter();
        }

        private string GetNullablePostfix(bool isNullable)
        {
            return isNullable && OutputTarget.GetProject().NullableEnabled ? "?" : "";
        }

        private bool IsPrimaryKey(IMetadataModel metadataModel)
        {
            var primaryKey = Model.GetPrimaryKeyAttribute();
            return metadataModel.Id == primaryKey.IdAttribute.Id;
        }

        private void AddAttributesBasedOnMetadataModel(CSharpProperty documentProperty, IMetadataModel entityPropertyMetadata)
        {
            if (IsPrimaryKey(entityPropertyMetadata))
            {
                documentProperty.AddAttribute(UseType("Redis.OM.Modeling.RedisIdField"));
            }

            switch (entityPropertyMetadata)
            {
                case AttributeModel attributeModel:
                    {
                        if (attributeModel.HasIndexed())
                        {
                            documentProperty.AddAttribute(UseType("Redis.OM.Modeling.Indexed"));
                        }
                        if (attributeModel.HasSearchable())
                        {
                            documentProperty.AddAttribute(UseType("Redis.OM.Modeling.Searchable"));
                        }
                        break;
                    }
                case AssociationTargetEndModel associationTargetEndModel:
                    if (associationTargetEndModel.HasIndexed())
                    {
                        documentProperty.AddAttribute(UseType("Redis.OM.Modeling.Indexed"), attr => attr.AddArgument("CascadeDepth = 1"));
                    }
                    break;
            }
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