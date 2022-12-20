using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Api.Indexes;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class EntityTypeConfigurationTemplate : CSharpTemplateBase<ClassModel, ITemplateDecorator>, ICSharpFileBuilderTemplate
    {
        private IIntentTemplate _entityTemplate;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.EntityTypeConfiguration";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public EntityTypeConfigurationTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.EntityFrameworkCore(Project));
            AddTypeSource("Domain.Entity");
            AddTypeSource("Domain.ValueObject");

            CSharpFile = new CSharpFile(OutputTarget.GetNamespace(), "")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddUsing("Microsoft.EntityFrameworkCore.Metadata.Builders")
                .AddClass($"{Model.Name}Configuration", @class =>
                {
                    if (!TryGetTemplate("Domain.Entity.State", Model, out _entityTemplate))
                    {
                        _entityTemplate = GetTemplate<IIntentTemplate>("Domain.Entity", Model);
                    }
                    @class.ImplementsInterface($"IEntityTypeConfiguration<{GetTypeName(_entityTemplate)}>")
                        .AddMethod("void", "Configure", method =>
                        {
                            method.AddMetadata("model", Model.InternalElement);
                            method.AddParameter($"EntityTypeBuilder<{GetTypeName(_entityTemplate)}>", "builder");
                            if (ForCosmosDb())
                            {
                                method.AddStatements(GetCosmosContainerMapping(Model));
                            }
                            method.AddStatements(GetTypeConfiguration(Model.InternalElement, @class));
                            method.AddStatements(GetCheckConstraints(Model));
                            method.AddStatements(GetIndexes(Model));
                            if (_entityTemplate is ICSharpFileBuilderTemplate builderTemplate)
                            {
                                // GCB - this approach (using the properties) is potentially worth exploring as it decouples the EF Core from the Domain designer
                                //builderTemplate.CSharpFile.OnBuild(file =>
                                //{
                                //    foreach (var property in file.Classes.First().GetAllProperties())
                                //    {
                                //        if (property.TryGetMetadata<AttributeModel>("model", out var attribute))
                                //        {
                                //            method.AddStatement(GetAttributeMapping(attribute, @class));
                                //        }
                                //        else if (property.TryGetMetadata<AssociationEndModel>("model", out var associationEnd))
                                //        {
                                //            method.AddStatement(GetAssociationMapping(associationEnd, @class));
                                //        }
                                //    }
                                //    method.Statements.SeparateAll();
                                //});

                                builderTemplate.CSharpFile.AfterBuild(file => // Needs to run after other decorators of the entity
                                {
                                    foreach (var property in file.Classes.First().GetAllProperties())
                                    {
                                        if (property.TryGetMetadata<bool>("non-persistent", out var nonPersistent) && nonPersistent &&
                                            !ParentConfigurationExists(Model))
                                        {
                                            method.AddStatement($"builder.Ignore(e => e.{property.Name});");
                                        }
                                    }
                                    method.Statements.SeparateAll();
                                });
                            }

                            method.Statements.SeparateAll();
                        });

                    foreach (var statement in @class.Methods.SelectMany(x => x.Statements.OfType<EfCoreKeyMappingStatement>().Where(x => x.KeyColumns.Any())))
                    {
                        EnsurePrimaryKeysOnEntity(
                            statement.KeyColumns.First().Class,
                            statement.KeyColumns);
                    }

                    foreach (var statement in @class.Methods.SelectMany(x => x.Statements.OfType<EfCoreAssociationConfigStatement>().Where(x => x.RequiredProperties.Any())))
                    {
                        EnsureForeignKeysOnEntity(
                            statement.RequiredProperties.First().Class,
                            statement.RequiredProperties);
                    }
                });
        }

        public CSharpFile CSharpFile { get; }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Configuration",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(new EntityTypeConfigurationCreatedEvent(this));
        }

        private bool ForCosmosDb()
        {
            return ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos();
        }

        private IEnumerable<CSharpStatement> GetTypeConfiguration(IElement targetType, CSharpClass @class)
        {
            var statements = new List<CSharpStatement>();

            if (targetType.IsClassModel())
            {
                if (!ForCosmosDb())
                {
                    statements.AddRange(GetTableMapping(targetType.AsClassModel()));
                }
                statements.AddRange(GetKeyMappings(targetType.AsClassModel()));
            }

            statements.AddRange(GetAttributes(targetType)
                .Where(RequiresConfiguration)
                .Select(x => GetAttributeMapping(x, @class)));

            statements.AddRange(GetAssociations(targetType)
                .Where(RequiresConfiguration)
                .Select(x => GetAssociationMapping(x, @class)));

            return statements.Where(x => x != null).ToList();
        }

        private IEnumerable<CSharpStatement> GetTableMapping(ClassModel model)
        {
            if (model.IsAggregateRoot())
            {
                if (model.HasTable() && (model.ParentClass != null || !string.IsNullOrWhiteSpace(model.GetTable().Name()) || !string.IsNullOrWhiteSpace(model.GetTable().Schema())))
                {
                    yield return $@"builder.ToTable(""{model.GetTable()?.Name() ?? model.Name.Pluralize()}""{(!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()) ? @$", ""{model.GetTable().Schema()}""" : "")});";
                }
                else if (ParentConfigurationExists(model))
                {
                    yield return $@"builder.HasBaseType<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, model.ParentClass)}>();";
                }
            }
            else if (model.HasTable() && (model.ParentClass != null || !string.IsNullOrWhiteSpace(model.GetTable().Name()) || !string.IsNullOrWhiteSpace(model.GetTable().Schema())))
            {
                yield return $@"builder.ToTable(""{model.GetTable()?.Name() ?? model.Name.Pluralize()}""{(!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()) ? @$", ""{model.GetTable().Schema()}""" : "")});";
            }
        }

        private IEnumerable<CSharpStatement> GetCosmosContainerMapping(ClassModel model)
        {
            // Is there an easier way to get this?
            var domainPackage = new DomainPackageModel(model.InternalElement.Package);
            var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

            if (model.ParentClass == null)
            {
                var containerName = string.IsNullOrWhiteSpace(cosmosSettings?.ContainerName())
                    ? ExecutionContext.GetApplicationConfig().Name
                    : cosmosSettings.ContainerName();

                yield return $@"builder.ToContainer(""{containerName}"");";
            }
            else if (ParentConfigurationExists(model))
            {
                yield return $"builder.HasBaseType<{GetTypeName(model.ParentClass.InternalElement)}>();";
            }

            if (GetPartitionKey(model) != null)
            {
                yield return $@"builder.HasPartitionKey(x => x.{GetPartitionKey(model).Name.ToPascalCase()});";
            }
            else
            {
                yield return $@"builder.HasPartitionKey(x => x.Id);";
            }
        }

        private CSharpStatement GetAttributeMapping(AttributeModel attribute, CSharpClass @class)
        {
            if (!IsOwned(attribute.TypeReference.Element))
            {
                return EfCoreFieldConfigStatement.CreateProperty(attribute, ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum());
            }

            @class.AddMethod("void", $"Configure{attribute.Name.ToPascalCase()}", method =>
            {
                method.AddMetadata("model", attribute.TypeReference.Element);
                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(attribute.InternalElement.ParentElement)}, {GetTypeName((IElement)attribute.TypeReference.Element)}>", "builder");
                method.AddStatements(GetTypeConfiguration((IElement)attribute.TypeReference.Element, @class).ToArray());
                method.Statements.SeparateAll();
            });

            return attribute.TypeReference.IsCollection
                ? EfCoreFieldConfigStatement.CreateOwnsMany(attribute)
                : EfCoreFieldConfigStatement.CreateOwnsOne(attribute);
        }

        private CSharpStatement GetAssociationMapping(AssociationEndModel associationEnd, CSharpClass @class)
        {
            if (associationEnd.Element.Id.Equals(associationEnd.OtherEnd().Element.Id)
                && associationEnd.Name.Equals(associationEnd.Element.Name))
            {
                Logging.Log.Warning($"Self referencing relationship detected using the same name for the Association as the Class: {associationEnd.Class.Name}. This might cause problems.");
            }

            switch (associationEnd.Association.GetRelationshipType())
            {
                case RelationshipType.OneToOne:
                    if (IsOwned(associationEnd.Element))
                    {
                        var field = EfCoreAssociationConfigStatement.CreateOwnsOne(associationEnd);
                        @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                        {
                            var sourceType = Model.IsSubclassOf(associationEnd.OtherEnd().Class) ? Model.InternalElement : (IElement)associationEnd.OtherEnd().Element;
                            method.AddMetadata("model", (IElement)associationEnd.Element);
                            method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(sourceType)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                            method.AddStatement(field.CreateWithOwner().WithForeignKey(associationEnd.Element.IsClassModel()));
                            method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class).ToArray());
                            method.Statements.SeparateAll();
                        });

                        return field;
                    }

                    return EfCoreAssociationConfigStatement.CreateHasOne(associationEnd)
                        .WithForeignKey();

                case RelationshipType.ManyToOne:
                    return EfCoreAssociationConfigStatement.CreateHasOne(associationEnd)
                        .WithForeignKey();

                case RelationshipType.OneToMany:
                    {
                        if (IsOwned(associationEnd.Element))
                        {
                            var field = EfCoreAssociationConfigStatement.CreateOwnsMany(associationEnd);
                            @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                            {
                                var sourceType = Model.IsSubclassOf(associationEnd.OtherEnd().Class) ? Model.InternalElement : (IElement)associationEnd.OtherEnd().Element;
                                method.AddMetadata("model", (IElement)associationEnd.Element);
                                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(sourceType)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                                method.AddStatement(field.CreateWithOwner().WithForeignKey(!ForCosmosDb() && associationEnd.Element.IsClassModel()));
                                method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class).ToArray());
                                method.Statements.SeparateAll();
                            });

                            return field;
                        }
                    }
                    return EfCoreAssociationConfigStatement.CreateHasMany(associationEnd)
                        .WithForeignKey();

                case RelationshipType.ManyToMany:
                    return EfCoreAssociationConfigStatement.CreateHasMany(associationEnd);
                default:
                    throw new Exception($"Relationship type for association [{Model.Name}.{associationEnd.Name}] could not be determined.");
            }
        }

        private bool RequiresConfiguration(AttributeModel attribute)
        {
            return !attribute.InternalElement.ParentElement.IsClassModel() ||
                   (attribute.Class.GetExplicitPrimaryKey().All(key => !key.Equals(attribute)) &&
                   !attribute.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase));
        }

        private bool RequiresConfiguration(AssociationEndModel associationEnd)
        {
            return associationEnd.IsTargetEnd();
        }

        private AttributeModel GetPartitionKey(ClassModel model)
        {
            return model.GetTypesInHierarchy().SelectMany(x => x.Attributes).SingleOrDefault(p => p.HasPartitionKey());
        }

        private IEnumerable<string> GetCheckConstraints(ClassModel model)
        {
            var checkConstraints = model.GetCheckConstraints();
            foreach (var checkConstraint in checkConstraints)
            {
                yield return @$"builder.HasCheckConstraint(""{checkConstraint.Name()}"", ""{checkConstraint.SQL()}"");";
            }
        }

        private CSharpStatement[] GetIndexes(ClassModel model)
        {
            var indexes = model.GetIndexes();
            if (indexes.Count == 0)
            {
                return Array.Empty<CSharpStatement>();
            }

            var statements = new List<string>();

            foreach (var index in indexes)
            {
                var indexFields = index.KeyColumns.Length == 1
                    ? GetIndexColumnPropertyName(index.KeyColumns.Single(), "x.")
                    : $"new {{ {string.Join(", ", index.KeyColumns.Select(x => GetIndexColumnPropertyName(x, "x.")))} }}";

                var sb = new StringBuilder($@"builder.HasIndex(x => {indexFields})");

                if (index.IncludedColumns.Length > 0)
                {
                    sb.Append($@"
                .IncludeProperties(x => new {{ {string.Join(", ", index.IncludedColumns.Select(x => GetIndexColumnPropertyName(x, "x.")))} }})");
                }

                switch (index.FilterOption)
                {
                    case FilterOption.Default:
                        break;
                    case FilterOption.None:
                        sb.Append(@"
                .HasFilter(null)");
                        break;
                    case FilterOption.Custom:
                        sb.Append(@$"
                .HasFilter(\""{index.Filter}"")");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (index.IsUnique)
                {
                    sb.Append(@"
                .IsUnique()");
                }

                if (!index.UseDefaultName)
                {
                    sb.Append(@$"
                .HasDatabaseName(""{index.Name}"")");
                }

                sb.Append(";");

                statements.Add(sb.ToString());
            }

            return statements.Select(x => new CSharpStatement(x)).ToArray();
        }

        private static string GetIndexColumnPropertyName(IndexColumn column, string prefix = null)
        {
            return column.SourceType.IsAssociationEndModel()
                ? $"{prefix}{column.Name.ToPascalCase()}Id"
                : $"{prefix}{column.Name.ToPascalCase()}";
        }

        public IEnumerable<CSharpStatement> GetKeyMappings(ClassModel model)
        {
            if (model.ParentClass != null && ParentConfigurationExists(model))
            {
                yield break;
            }

            yield return new EfCoreKeyMappingStatement(model);

            foreach (var attributeModel in model.GetExplicitPrimaryKey().Where(x =>
                         !string.IsNullOrWhiteSpace(x.GetColumn()?.Name()) ||
                         !string.IsNullOrWhiteSpace(x.GetColumn()?.Type())))
            {
                yield return new EfCoreKeyColumnPropertyStatement(attributeModel);
            }
        }

        protected string GetDefaultSurrogateKeyType()
        {
            return GetDefaultSurrogateKeyType(ExecutionContext);
        }

        protected static string GetDefaultSurrogateKeyType(ISoftwareFactoryExecutionContext executionContext)
        {
            var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
            switch (settingType)
            {
                case "guid":
                    return "System.Guid";
                case "int":
                    return "int";
                case "long":
                    return "long";
                default:
                    return settingType;
            }
        }

        public void EnsurePrimaryKeysOnEntity(ICanBeReferencedType entityModel, params RequiredEntityProperty[] columns)
        {
            if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, entityModel.Id, out var template))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var entityClass = file.Classes.First();
                    var primaryKeyProperties = new List<CSharpProperty>();
                    foreach (var column in columns)
                    {
                        var existingPk = entityClass.GetAllProperties().FirstOrDefault(x => x.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase));
                        if (existingPk == null)
                        {
                            var typeName = column.Type != null
                                ? template.GetTypeName(column.Type.AsTypeReference(isNullable: column.IsNullable, isCollection: column.IsCollection))
                                : this.GetDefaultSurrogateKeyType() + (column.IsNullable ? "?" : string.Empty);

                            entityClass.InsertProperty(0, template.UseType(typeName), column.Name, property =>
                            {
                                column.ConfigureProperty?.Invoke(property);
                                primaryKeyProperties.Add(property);
                            });
                        }
                        else
                        {
                            primaryKeyProperties.Add(existingPk);
                        }
                    }

                    if (!entityClass.TryGetMetadata("primary-keys", out var pks))
                    {
                        entityClass.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                    }
                });
            }
        }

        public void EnsureForeignKeysOnEntity(ICanBeReferencedType entityModel, params RequiredEntityProperty[] columns)
        {
            if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, entityModel.Id, out var template))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var entityClass = file.Classes.First();
                    foreach (var column in columns)
                    {
                        if (entityClass.GetAllProperties().All(x => !x.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var typeName = column.Type != null
                                ? template.GetTypeName(column.Type.AsTypeReference(isNullable: column.IsNullable, isCollection: column.IsCollection))
                                : this.GetDefaultSurrogateKeyType() + (column.IsNullable ? "?" : string.Empty);

                            var associationProperty = entityClass.Properties.SingleOrDefault(x => x.Name.Equals(column.Name.RemoveSuffix("Id")));
                            if (associationProperty != null)
                            {
                                entityClass.InsertProperty(entityClass.Properties.IndexOf(associationProperty), template.UseType(typeName), column.Name, column.ConfigureProperty);
                            }
                            else
                            {
                                entityClass.AddProperty(template.UseType(typeName), column.Name, column.ConfigureProperty);
                            }
                        }
                    }
                });
            }
        }

        private bool ParentConfigurationExists(ClassModel model)
        {
            return model.ParentClass != null && TryGetTemplate<EntityTypeConfigurationTemplate>(Id, model.ParentClass?.Id, out _);
        }

        private bool IsOwned(ICanBeReferencedType type)
        {
            return type.IsOwned(ExecutionContext);
        }

        private IEnumerable<AttributeModel> GetAttributes(IElement model)
        {
            var attributes = new List<AttributeModel>();
            var @class = model.AsClassModel();
            if (@class?.ParentClass != null && !ParentConfigurationExists(@class))
            {
                attributes.AddRange(GetAttributes(@class.ParentClass.InternalElement));
            }

            attributes.AddRange(model.ChildElements
                .Where(x => x.IsAttributeModel() && RequiresConfiguration(x.AsAttributeModel()))
                .Select(x => x.AsAttributeModel())
                .ToList());
            return attributes;
        }

        private IEnumerable<AssociationEndModel> GetAssociations(IElement model)
        {
            var associations = new List<AssociationEndModel>();
            var @class = model.AsClassModel();
            if (@class?.ParentClass != null && !ParentConfigurationExists(@class))
            {
                associations.AddRange(GetAssociations(@class.ParentClass.InternalElement));
            }

            associations.AddRange(model.AssociatedElements
                .Where(x => x.IsAssociationEndModel() && RequiresConfiguration(x.AsAssociationEndModel()))
                .Select(x => x.AsAssociationEndModel())
                .ToList());
            return associations;
        }
    }
}