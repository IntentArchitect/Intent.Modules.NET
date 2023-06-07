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
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
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
                            method.AddStatements(GetTypeConfiguration(Model.InternalElement, @class));
                            method.AddStatements(GetCheckConstraints(Model));
                            method.AddStatements(GetIndexes(Model));
                            method.Statements.SeparateAll();

                            AddIgnoreForNonPersistent(method, isOwned: false);
                        });

                    EnsureParameterlessConstructorOnEntity(Model);

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
                    (!TryGetTemplate("Domain.Entity.State", element, out ICSharpFileBuilderTemplate entityTemplate) &&
                     !TryGetTemplate("Domain.Entity", element, out entityTemplate)))
                {
                    return;
                }

                var classModel = element.AsClassModel();
                var @class = entityTemplate.CSharpFile.Classes.First();

                foreach (var property in @class.GetAllProperties())
                {
                    if (property.TryGetMetadata("non-persistent", out bool nonPersistent) && nonPersistent &&
                        (isOwned || !IsInheriting(classModel) || !ParentConfigurationExists(classModel)))
                    {
                        method.AddStatement($"builder.Ignore(e => e.{property.Name});");
                    }
                }

                method.Statements.SeparateAll();
            });
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

        private IEnumerable<CSharpStatement> GetTypeConfiguration(IElement targetType, CSharpClass @class)
        {
            var statements = new List<CSharpStatement>();

            if (targetType.IsClassModel())
            {
                var classModel = new ClassExtensionModel(targetType);

                if (!ForCosmosDb())
                {
                    statements.AddRange(GetTableMapping(classModel));
                }

                statements.AddRange(GetKeyMappings(classModel));
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
                .Select(x => GetAttributeMapping(x, @class)));

            statements.AddRange(GetAssociations(targetType)
                .Where(RequiresConfiguration)
                .Select(x => GetAssociationMapping(x, @class)));

            return statements.Where(x => x != null).ToList();
        }

        private bool HasSerializationSupport()
        {
            var proj = OutputTarget.GetProject();
            return proj switch
            {
                _ when proj.IsNetCore2App() => false,
                _ when proj.IsNetCore3App() => false,
                _ when proj.IsNetApp(4) => false,
                _ when proj.IsNetApp(5) => false,
                _ when proj.IsNetApp(6) => false,
                _ => true // Only .NET 7+ supports this (safe to assume EF Core 7 for .NET 7)
            };
        }

        private static bool HasSerializationType(ICSharpFileBuilderTemplate valueObjectTemplate, out string serializationType)
        {
            return valueObjectTemplate.CSharpFile.Classes.First().TryGetMetadata<string>("serialization", out serializationType);
        }

        private IEnumerable<CSharpStatement> GetTableMapping(ClassExtensionModel model)
        {
            if (model.HasView() && model.HasTable())
            {
                throw new Exception($"Class \"{model.Name}\" [{model.Id}] has both a \"Table\" and \"View\" stereotype applied to it.");
            }

            if (model.HasView())
            {
                yield return $@"builder.ToView(""{model.GetView()?.Name() ?? model.Name.Pluralize()}""{(!string.IsNullOrWhiteSpace(model.GetView()?.Schema()) ? @$", ""{model.GetView().Schema()}""" : "")});";
            }
            else if (model.HasTable() && (IsInheriting(model) || !string.IsNullOrWhiteSpace(model.GetTable().Name()) || !string.IsNullOrWhiteSpace(model.GetTable().Schema())))
            {
                yield return ToTableStatement(model);
            }
            else if (model.IsAggregateRoot() && IsInheriting(model) && ParentConfigurationExists(model))
            {
                if (model.Triggers.Any())
                {
                    yield return ToTableStatement(model);
                }

                yield return $@"builder.HasBaseType<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, model.ParentClass)}>();";
            }
            else if (model.Triggers.Any())
            {
                yield return ToTableStatement(model);
            }

            static CSharpStatement ToTableStatement(ClassExtensionModel model)
            {
                var statement = new CSharpInvocationStatement("builder.ToTable");
                if (!string.IsNullOrWhiteSpace(model.GetTable()?.Name()) ||
                    !string.IsNullOrWhiteSpace(model.GetTable()?.Schema()))
                {
                    statement.AddArgument($"\"{model.GetTable()?.Name() ?? model.Name.Pluralize()}\"");
                }

                if (!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()))
                {
                    statement.AddArgument($"\"{model.GetTable().Schema()}\"");
                }

                if (model.Triggers.Count == 1)
                {
                    statement.AddArgument($"tb => tb.HasTrigger(\"{model.Triggers[0].Name}\")");
                }
                else if (model.Triggers.Count > 1)
                {
                    statement.WithArgumentsOnNewLines();

                    var lambda = new CSharpLambdaBlock("tb");
                    foreach (var trigger in model.Triggers)
                    {
                        lambda.AddStatement($"tb.HasTrigger(\"{trigger.Name}\");");
                    }

                    statement.AddArgument(lambda);
                }

                return statement;
            }
        }

        private IEnumerable<CSharpStatement> GetCosmosContainerMapping(ClassModel model)
        {
            // Is there an easier way to get this?
            var domainPackage = new DomainPackageModel(model.InternalElement.Package);
            var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

            if (!IsInheriting(model) || !ParentConfigurationExists(model))
            {
                var containerName = string.IsNullOrWhiteSpace(cosmosSettings?.ContainerName())
                    ? ExecutionContext.GetApplicationConfig().Name
                    : cosmosSettings.ContainerName();

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

                AddIgnoreForNonPersistent(method, isOwned: true);
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

                            AddIgnoreForNonPersistent(method, isOwned: true);
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

                                AddIgnoreForNonPersistent(method, isOwned: true);
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
            var databaseProvider = ExecutionContext.Settings.GetDatabaseSettings()?.DatabaseProvider()?.AsEnum();
            var dbSupportsIncludedProperties = databaseProvider switch
            {
                DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer => true,
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
                : $"{prefix}{column.Name.ToPascalCase()}";
        }

        public IEnumerable<CSharpStatement> GetKeyMappings(ClassModel model)
        {
            if (IsInheriting(model) && ParentConfigurationExists(model))
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

        private void EnsureParameterlessConstructorOnEntity(ClassModel model)
        {
            if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, model.Id, out var template))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var entityClass = file.Classes.First();
                    if (entityClass.Constructors.Count > 0 &&
                        entityClass.Constructors.All(x => x.Parameters.Count > 0))
                    {
                        entityClass.AddConstructor(ctor =>
                        {
                            ctor.WithComments(new[]
                            {
                                "/// <summary>",
                                "/// Required by Entity Framework.",
                                "/// </summary>"
                            });
                            ctor.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            ctor.Protected();
                            foreach (var attribute in model.Attributes)
                            {
                                if (string.IsNullOrWhiteSpace(attribute.Value))
                                {
                                    var typeInfo = GetTypeInfo(attribute.TypeReference);
                                    if (NeedsNullabilityAssignment(typeInfo))
                                    {
                                        ctor.AddStatement($"{attribute.Name.ToPascalCase()} = null!;");
                                    }
                                }
                            }
                        });
                    }
                });
            }
        }

        private bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
        {
            return !(typeInfo.IsPrimitive 
                || typeInfo.IsNullable == true 
                || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.IsEnumModel()));
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
                        if (entityClass.GetAllProperties().Any(prop => prop.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            continue;
                        }

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
                });
            }
        }

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