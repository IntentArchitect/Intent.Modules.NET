using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Api.Indexes;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using ClassExtensionModel = Intent.Metadata.RDBMS.Api.ClassExtensionModel;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    [IntentManaged(Mode.Merge)]
    public partial class EntityTypeConfigurationTemplate : CSharpTemplateBase<ClassModel, ITemplateDecorator>, ICSharpFileBuilderTemplate
    {
        private IIntentTemplate _entityTemplate;
        private readonly bool _enforceColumnOrdering;
        private int _columnCurrentOrder;

        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.EntityFrameworkCore.EntityTypeConfiguration";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public EntityTypeConfigurationTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            _enforceColumnOrdering = ExecutionContext.Settings.GetDatabaseSettings().MaintainColumnOrdering();

            AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCore(Project));
            AddTypeSource("Domain.Entity");
            AddTypeSource("Domain.ValueObject");
            AddTypeSource("Intent.Entities.DomainEnum");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
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

                            method.AddStatements(GetTypeConfiguration(Model.InternalElement, @class, null));
                            //Moved this into GetTypeConfiguration as its applicabale for Owned Tables too
                            //method.AddStatements(GetCheckConstraints(Model));
                            method.Statements.SeparateAll();

                            AddIgnoreForNonPersistent(method, isOwned: false);
                        });

                    foreach (var statement in @class.Methods.SelectMany(x => x.Statements.OfType<EfCoreKeyMappingStatement>().Where(y => y.KeyColumns.Any())))
                    {
                        EnsurePrimaryKeysOnEntity(
                            statement.KeyColumns.First().Class,
                            statement.KeyColumns);
                    }
                });
        }

        private void AddIgnoreForNonPersistent(CSharpClassMethod method, bool isOwned)
        {
            if (_entityTemplate is not ICSharpFileBuilderTemplate entityBuilder)
            {
                return;
            }

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

            entityBuilder.CSharpFile.AfterBuild(_ => // Needs to run after other decorators of the entity
            {
                if (!method.TryGetMetadata("model", out IElement element) ||
                    (!TryGetTemplate("Domain.Entity.State", element, out ICSharpFileBuilderTemplate _) &&
                     !TryGetTemplate("Domain.Entity", element, out ICSharpFileBuilderTemplate _)))
                {
                    return;
                }

                var classModel = element.AsClassModel();

                foreach (var property in GetAllBuilderProperties(classModel))
                {
                    if (property.TryGetMetadata("non-persistent", out bool nonPersistent) && nonPersistent &&
                        !isOwned && !HasInheritanceTypeAbleToConfigureProperty(classModel))
                    {
                        method.AddStatement($"builder.Ignore(e => e.{property.Name});");
                    }
                }

                method.Statements.SeparateAll();
            });
        }

        private bool HasInheritanceTypeAbleToConfigureProperty(ClassModel currentModel)
        {
            if (currentModel.ParentClass is null)
            {
                return false;
            }

            if (ConfigurationExists(currentModel.ParentClass))
            {
                return true;
            }

            return HasInheritanceTypeAbleToConfigureProperty(currentModel.ParentClass);
        }

        private IEnumerable<CSharpProperty> GetAllBuilderProperties(ClassModel targetClass)
        {
            if (targetClass is null)
            {
                return [];
            }

            CSharpClass @class;
            if (TryGetTemplate("Domain.Entity.State", targetClass, out ICSharpFileBuilderTemplate entityTemplate))
            {
                @class = entityTemplate.CSharpFile.Classes.First();
            }
            else if (TryGetTemplate("Domain.Entity", targetClass, out entityTemplate))
            {
                @class = entityTemplate.CSharpFile.Classes.First();
            }
            else
            {
                return [];
            }

            return @class.Properties.Concat(GetAllBuilderProperties(targetClass.ParentClass)).ToArray();
        }

        public CSharpFile CSharpFile { get; }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
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

        private IEnumerable<CSharpStatement> GetTypeConfiguration(IElement targetType, CSharpClass @class, RelationshipType? ownedRelationship)
        {
            var statements = new List<CSharpStatement>();

            if (targetType.IsClassModel())
            {
                var classModel = new ClassExtensionModel(targetType);

                if (!ForCosmosDb())
                {
                    statements.AddRange(GetTableMapping(classModel));
                }

                statements.AddRange(GetKeyMappings(classModel, ownedRelationship));
            }

            if (targetType.IsValueObject(ExecutionContext, out var valueObjectTemplate) &&
                HasSerializationType(valueObjectTemplate, out var serializationType) &&
                serializationType == "JSON" &&
                HasSerializationSupport())
            {
                statements.Add("builder.ToJson();");
            }

            statements.AddRange(GetAttributes(targetType)
                .Where(RequiresConfiguration)
                .Select(x => GetAttributeMapping(x, @class, ownedRelationship)));

            if (targetType.IsClassModel())
            {
                var classModel = new ClassExtensionModel(targetType);
                statements.AddRange(GetIndexes(classModel));
            }

            statements.AddRange(GetAssociations(targetType)
                .Where(RequiresConfiguration)
                .Select(x => GetAssociationMapping(x, targetType, @class)));

            if (targetType.IsClassModel())
            {
                statements.AddRange(GetCheckConstraints(targetType.AsClassModel()).Select(s => new CSharpStatement(s)));
            }

            return statements.Where(x => x != null).ToList();
        }

        private bool HasSerializationSupport()
        {
            // Only .NET 7+ supports this (safe to assume EF Core 7 for .NET 7)
            return OutputTarget.GetProject().TryGetMaxNetAppVersion(out var version) &&
                   version.Major >= 7;
        }

        private static bool HasSerializationType(ICSharpFileBuilderTemplate valueObjectTemplate, out string serializationType)
        {
            return valueObjectTemplate.CSharpFile.TypeDeclarations.First().TryGetMetadata("serialization", out serializationType);
        }

        private IEnumerable<CSharpStatement> GetTableMapping(ClassExtensionModel model)
        {
            if (model.HasView() && model.HasTable())
            {
                throw new Exception($"Class \"{model.Name}\" [{model.Id}] has both a \"Table\" and \"View\" stereotype applied to it.");
            }

            if (model.HasView())
            {
                yield return
                    $@"builder.ToView(""{model.GetView()?.Name() ?? GetTableNameByConvention(model.Name)}""{(!string.IsNullOrWhiteSpace(model.FindSchema()) ? @$", ""{model.FindSchema()}""" : "")});";
            }
            else if (model.HasTable() && (IsInheriting(model) || !string.IsNullOrWhiteSpace(model.GetTable().Name()) || !string.IsNullOrWhiteSpace(model.FindSchema())))
            {
                yield return ToTableStatement(model);
            }
            else if (model.IsAggregateRoot() && IsInheriting(model) && ParentConfigurationExists(model))
            {
                if (model.Triggers.Any())
                {
                    yield return ToTableStatement(model);
                }
                if (_enforceColumnOrdering)
                {
                    var currentParent = model.ParentClass;
                    while (currentParent != null)
                    {
                        _columnCurrentOrder = currentParent.Attributes.Count;
                        if (currentParent.IsAggregateRoot() && IsInheriting(currentParent) && ParentConfigurationExists(currentParent))
                        {
                            currentParent = currentParent.ParentClass;
                        }
                        else
                        {
                            currentParent = null;
                        }
                    }
                }

                yield return $@"builder.HasBaseType<{GetTypeName(TemplateRoles.Domain.Entity.Primary, model.ParentClass)}>();";
            }
            else if (model.Triggers.Any())
            {
                yield return ToTableStatement(model);
            }
            else if (!IsTableInlined(model) && (!string.IsNullOrEmpty(model.FindSchema()) || RequiresToTableStatementForConvention(model.Name)))
            {
                yield return ToTableStatement(model);
            }

            yield break;

            CSharpStatement ToTableStatement(ClassExtensionModel modelInner)
            {
                var schema = modelInner.FindSchema();
                var statement = new CSharpInvocationStatement("builder.ToTable");
                if (!string.IsNullOrWhiteSpace(modelInner.GetTable()?.Name()) ||
                    !string.IsNullOrWhiteSpace(schema) ||
                    modelInner.Triggers.Count == 0)
                {
                    statement.AddArgument($"\"{modelInner.GetTable()?.Name() ?? GetTableNameByConvention(modelInner.Name)}\"");
                }

                if (!string.IsNullOrWhiteSpace(schema))
                {
                    statement.AddArgument($"\"{schema}\"");
                }

                if (modelInner.Triggers.Count == 1)
                {
                    statement.AddArgument($"tb => tb.HasTrigger(\"{modelInner.Triggers[0].Name}\")");
                }
                else if (modelInner.Triggers.Count > 1)
                {
                    statement.WithArgumentsOnNewLines();

                    var lambda = new CSharpLambdaBlock("tb");
                    foreach (var trigger in modelInner.Triggers)
                    {
                        lambda.AddStatement($"tb.HasTrigger(\"{trigger.Name}\");");
                    }

                    statement.AddArgument(lambda);
                }

                return statement;
            }
        }

        private bool IsTableInlined(ClassExtensionModel model)
        {
            if (model.HasView() || model.HasTable())
            {
                return false;
            }
            if (!IsOwned(model.InternalElement))
            {
                return false;
            }
            if (model.InternalElement.IsClassModel())
            {
                var type = model.InternalElement.AsClassModel();
                var from = type.AssociatedFromClasses();
                if (from.Count == 1 && from.First().Association.GetRelationshipType() == RelationshipType.OneToOne)
                {
                    return true;
                }
            }
            return model.InternalElement.IsValueObject(ExecutionContext);
        }

        private bool RequiresToTableStatementForConvention(string className)
        {
            switch (ExecutionContext.Settings.GetDatabaseSettings().TableNamingConvention().AsEnum())
            {
                case DatabaseSettingsExtensions.TableNamingConventionOptionsEnum.Singularized:
                    return true;
                case DatabaseSettingsExtensions.TableNamingConventionOptionsEnum.None:
                    //Because DBSets are plural table names default to table, we need to add ToTables in the name is not pluralized
                    return className != className.Pluralize();
                case DatabaseSettingsExtensions.TableNamingConventionOptionsEnum.Pluralized:
                default:
                    return false;
            }
        }

        private string GetTableNameByConvention(string className)
        {
            switch (ExecutionContext.Settings.GetDatabaseSettings().TableNamingConvention().AsEnum())
            {
                case DatabaseSettingsExtensions.TableNamingConventionOptionsEnum.Singularized:
                    return className.Singularize();
                case DatabaseSettingsExtensions.TableNamingConventionOptionsEnum.None:
                    return className;
                case DatabaseSettingsExtensions.TableNamingConventionOptionsEnum.Pluralized:
                default:
                    return className.Pluralize();
            }
        }

        private IEnumerable<CSharpStatement> GetCosmosContainerMapping(ClassModel model)
        {
            // Is there an easier way to get this?
            var cosmosContainerName = GetNearestCosmosDbContainerName(model);

            if (!IsInheriting(model) || !ParentConfigurationExists(model))
            {
                var containerName = string.IsNullOrWhiteSpace(cosmosContainerName)
                    ? ExecutionContext.GetApplicationConfig().Name
                    : cosmosContainerName;

                yield return $@"builder.ToContainer(""{containerName}"");";
            }

            if (IsInheriting(model) && ParentConfigurationExists(model))
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

        private static string GetNearestCosmosDbContainerName(ClassModel model)
        {
            if (model.GetCosmosDBContainerSettings() is not null)
            {
                return model.GetCosmosDBContainerSettings().ContainerName();
            }

            var stereotype = model.GetStereotypeInFolders("Cosmos DB Container Settings");
            if (stereotype is not null)
            {
                return stereotype.GetProperty<string>("Container Name");
            }

            var domainPackage = new DomainPackageModel(model.InternalElement.Package);
            return domainPackage.GetCosmosDBContainerSettings()?.ContainerName();
        }

        private CSharpStatement GetAttributeMapping(AttributeModel attribute, CSharpClass @class, RelationshipType? ownedRelationship)
        {
            if (!IsOwned(attribute.TypeReference.Element))
            {
                return EfCoreFieldConfigStatement.CreateProperty(attribute, ExecutionContext.Settings.GetDatabaseSettings(), _enforceColumnOrdering ? _columnCurrentOrder++ : null);
            }

            @class.AddMethod("void", $"Configure{attribute.Name.ToPascalCase()}", method =>
            {
                method.AddMetadata("model", attribute.TypeReference.Element);
                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(attribute.InternalElement.ParentElement)}, {GetTypeName((IElement)attribute.TypeReference.Element)}>",
                    "builder");
                method.AddStatements(GetTypeConfiguration((IElement)attribute.TypeReference.Element, @class, ownedRelationship).ToArray());
                method.Statements.SeparateAll();

                AddIgnoreForNonPersistent(method, isOwned: true);
            });

            return attribute.TypeReference.IsCollection
                ? EfCoreFieldConfigStatement.CreateOwnsMany(attribute)
                : EfCoreFieldConfigStatement.CreateOwnsOne(attribute);
        }

        private CSharpStatement GetAssociationMapping(AssociationEndModel associationEnd, IElement targetType, CSharpClass @class)
        {
            if (associationEnd.Element.Id.Equals(associationEnd.OtherEnd().Element.Id)
                && associationEnd.Name.Equals(associationEnd.Element.Name))
            {
                Logging.Log.Warning(
                    $"Self referencing relationship detected using the same name for the Association as the Class: {associationEnd.Class.Name}. This might cause problems.");
            }

            switch (associationEnd.Association.GetRelationshipType())
            {
                case RelationshipType.OneToOne:
                    if (IsOwned(associationEnd.Element))
                    {
                        var field = EfCoreAssociationConfigStatement.CreateOwnsOne(associationEnd, targetType);
                        @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                        {
                            var sourceType = Model.IsSubclassOf(associationEnd.OtherEnd().Class) ? Model.InternalElement : (IElement)associationEnd.OtherEnd().Element;
                            method.AddMetadata("model", (IElement)associationEnd.Element);
                            method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(sourceType)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                            method.AddStatement(field.CreateWithOwner().WithForeignKey(!ForCosmosDb() && associationEnd.Element.IsClassModel()));
                            method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class, RelationshipType.OneToOne).ToArray());
                            method.Statements.SeparateAll();

                            AddIgnoreForNonPersistent(method, isOwned: true);
                        });

                        return field;
                    }

                    return EfCoreAssociationConfigStatement.CreateHasOne(associationEnd, targetType)
                        .WithForeignKey();

                case RelationshipType.ManyToOne:
                    return EfCoreAssociationConfigStatement.CreateHasOne(associationEnd, targetType)
                        .WithForeignKey();

                case RelationshipType.OneToMany:
                    {
                        if (IsOwned(associationEnd.Element))
                        {
                            var field = EfCoreAssociationConfigStatement.CreateOwnsMany(associationEnd, targetType);
                            @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                            {
                                var sourceType = Model.IsSubclassOf(associationEnd.OtherEnd().Class) ? Model.InternalElement : (IElement)associationEnd.OtherEnd().Element;
                                method.AddMetadata("model", (IElement)associationEnd.Element);
                                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(sourceType)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                                method.AddStatement(field.CreateWithOwner().WithForeignKey((!ForCosmosDb() || !field.HasDefaultAssociationSourceName()) && associationEnd.Element.IsClassModel()));
                                method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class, RelationshipType.OneToMany).ToArray());
                                method.Statements.SeparateAll();

                                AddIgnoreForNonPersistent(method, isOwned: true);
                            });

                            return field;
                        }
                    }
                    return EfCoreAssociationConfigStatement.CreateHasMany(associationEnd, targetType, GetTableNameByConvention)
                        .WithForeignKey();

                case RelationshipType.ManyToMany:
                    return EfCoreAssociationConfigStatement.CreateHasMany(associationEnd, targetType, GetTableNameByConvention);
                default:
                    throw new Exception($"Relationship type for association [{Model.Name}.{associationEnd.Name}] could not be determined.");
            }
        }

        private static bool RequiresConfiguration(AttributeModel attribute)
        {
            return !attribute.InternalElement.ParentElement.IsClassModel() ||
                   attribute.Class.GetExplicitPrimaryKey().All(keyAttribute => !keyAttribute.Equals(attribute));
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
            if (ExecutionContext.Settings.GetDatabaseSettings().EnumCheckConstraints() && model.Attributes.Any(a => a.TypeReference.Element.IsEnumModel()))
            {
                var attributesToAdd = model.Attributes.Where(a => a.TypeReference.Element.IsEnumModel());
                foreach (var enumAttribute in attributesToAdd)
                {
                    AddUsing("System");
                    AddUsing("System.Linq");
                    var constraint = new StringBuilder();
                    var enumType = GetTypeName(enumAttribute.TypeReference);
                    var constraintValuesExpression = ExecutionContext.Settings.GetDatabaseSettings().StoreEnumsAsStrings()
                        ? $"Enum.GetValues<{enumType}>().Select(e => $\"'{{e}}'\")"
                        : $"Enum.GetValues<{enumType}>().Select(e => $\"{{e:D}}\")";

                    constraint.AppendLine();
                    constraint.AppendLine(@$"builder.ToTable(tb => tb.HasCheckConstraint(""{GetConstraintName(enumAttribute)}"", $""\""{enumAttribute.Name}\"" IN ({{string.Join("","", {constraintValuesExpression})}})""));");
                    yield return constraint.ToString();
                }
            }
            var checkConstraints = model.GetCheckConstraints();
            foreach (var checkConstraint in checkConstraints)
            {
                yield return $"""builder.HasCheckConstraint("{checkConstraint.Name()}", "{checkConstraint.SQL()}");""";
            }
        }

        private static string GetConstraintName(AttributeModel Model)
        {
            return (Model.InternalElement.ParentElement.Name + Model.Name + "Check").ToSnakeCase();
        }

        private CSharpStatement[] GetIndexes(ClassModel model)
        {
            var indexes = model.GetIndexes();
            if (indexes.Count == 0)
            {
                return [];
            }

            var statements = new List<string>();
            var databaseProvider = ExecutionContext.Settings.GetDatabaseSettings()?.DatabaseProvider()?.AsEnum();
            var dbSupportsIncludedProperties = databaseProvider switch
            {
                DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer => true,
                DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql => true,
                _ => false
            };

            if (!dbSupportsIncludedProperties && indexes.Any(i => i.IncludedColumns.Any()))
            {
                Logging.Log.Warning($"{model.Name} has one or more indexes with \"Included\" columns which is unsupported by the selected database provider ({databaseProvider}).");
            }

            foreach (var index in indexes)
            {
                var indexFields = index.KeyColumns.Length == 1
                    ? GetIndexColumnPropertyName(index.KeyColumns.Single(), "x.")
                    : $"new {{ {string.Join(", ", index.KeyColumns.Select(x => GetIndexColumnPropertyName(x, "x.")))} }}";

                var sb = new StringBuilder($@"builder.HasIndex(x => {indexFields})");

                if (index.KeyColumns.Any(x => x.SortDirection == SortDirection.Descending))
                {
                    sb.Append($@"
                .IsDescending({string.Join(", ", index.KeyColumns.Select(x => x.SortDirection == SortDirection.Ascending ? "false" : "true"))})");
                }

                if (dbSupportsIncludedProperties && index.IncludedColumns.Length > 0)
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
                .HasFilter(""{index.Filter.Replace("\"", "\\\"")}"")");
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

                if (index.FillFactor.HasValue)
                {
                    sb.Append($@"
                .HasFillFactor({index.FillFactor.Value})");
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
                : $"{prefix}{column.SourceType.Name.ToPascalCase()}";
        }

        public IEnumerable<CSharpStatement> GetKeyMappings(ClassModel model, RelationshipType? ownedRelationship)
        {
            if (HasInheritanceTypeAbleToConfigureProperty(model))
            {
                yield break;
            }

            if (!(ForCosmosDb() && !model.IsAggregateRoot() && !model.GetExplicitPrimaryKey().Any() && ownedRelationship == RelationshipType.OneToOne))
            {
                yield return new EfCoreKeyMappingStatement(model);
            }

            foreach (var attributeModel in model.GetExplicitPrimaryKey())
            {
                if (EfCoreKeyColumnPropertyStatement.RequiresConfiguration(attributeModel) || _enforceColumnOrdering)
                {
                    yield return new EfCoreKeyColumnPropertyStatement(attributeModel, _enforceColumnOrdering ? _columnCurrentOrder++ : null);
                }
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
            if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, entityModel.Id, out var template))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var entityClass = file.Classes.First();
                    var primaryKeyProperties = new List<CSharpProperty>();
                    foreach (var column in columns)
                    {
                        entityClass.TryGetMetadata<ClassModel>("model", out var model);
                        if (model is null)
                        {
                            continue;
                        }

                        var existingPk = GetAllBuilderProperties(model)
                            .FirstOrDefault(x => x.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase));

                        if (existingPk is null)
                        {
                            continue;
                        }

                        primaryKeyProperties.Add(existingPk);
                    }

                    if (!entityClass.TryGetMetadata("primary-keys", out _))
                    {
                        entityClass.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                    }
                });
            }
        }

        private bool ConfigurationExists(ClassModel model) => TryGetTemplate<EntityTypeConfigurationTemplate>(Id, model?.Id, out _);
        private bool ParentConfigurationExists(ClassModel model) => TryGetTemplate<EntityTypeConfigurationTemplate>(Id, model.ParentClass?.Id, out _);

        private static bool IsInheriting(ClassModel model) => model?.ParentClass != null;

        private bool IsOwned(ICanBeReferencedType type)
        {
            return type.IsOwned(ExecutionContext);
        }

        private IEnumerable<AttributeModel> GetAttributes(IElement model)
        {
            var attributes = new List<AttributeModel>();
            var @class = model.AsClassModel();
            if (IsInheriting(@class) && !ParentConfigurationExists(@class))
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
            if (IsInheriting(@class) && !ParentConfigurationExists(@class))
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