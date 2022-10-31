using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.RDBMS.Api;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.RDBMS.Api.Indexes;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
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

            Strategy = new RdbmsEntityTypeConfiguration(this);
            if (ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                Strategy = new CosmosEntityTypeConfiguration(this);
            }

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
                            method.AddStatements(Strategy.GetTableMapping(Model));
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
                                            !TryGetTemplate<EntityTypeConfigurationTemplate>(Id, Model.ParentClass?.Id, out var template))
                                        {
                                            method.AddStatement($"builder.Ignore(e => e.{property.Name});");
                                        }
                                    }
                                    method.Statements.SeparateAll();
                                });
                            }

                            method.Statements.SeparateAll();
                        });
                });
        }

        public CSharpFile CSharpFile { get; }
        public IEntityTypeConfigurationStrategy Strategy { get; set; }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(new EntityTypeConfigurationCreatedEvent(this));
        }

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

        private bool RequiresConfiguration(AttributeModel attribute)
        {
            return true;
            //return attribute.InternalElement.ParentElement.AsClassModel()?.GetExplicitPrimaryKey().All(key => !key.Equals(attribute)) == true &&
            //       !attribute.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool RequiresConfiguration(AssociationEndModel associationEnd)
        {
            return associationEnd.IsTargetEnd();
        }

        private EFCoreConfigStatementBase GetAttributeMapping(AttributeModel attribute, CSharpClass @class)
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

            if (attribute.TypeReference.IsCollection)
            {
                return EfCoreFieldConfigStatement.CreateOwnsMany(attribute);
            }
            else
            {
                return EfCoreFieldConfigStatement.CreateOwnsOne(attribute);
            }
        }

        private EFCoreConfigStatementBase GetAssociationMapping(AssociationEndModel associationEnd, CSharpClass @class)
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
                            method.AddStatement(field.CreateWithOwner().WithForeignKey());
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
                                method.AddStatement(field.CreateWithOwner().WithForeignKey());
                                method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class).ToArray());
                                method.Statements.SeparateAll();
                            });

                            return field;
                        }
                    }
                    return EfCoreAssociationConfigStatement.CreateHasMany(associationEnd)
                        .WithForeignKey();
                case RelationshipType.ManyToMany:
                    EnsureColumnsOnEntity(associationEnd.Element, new RequiredColumn(_entityTemplate.GetTypeName(associationEnd.OtherEnd()), associationEnd.OtherEnd().Name.ToPascalCase(),
                        property =>
                        {
                            property.Protected().Virtual();
                        }));
                    return EfCoreAssociationConfigStatement.CreateHasMany(associationEnd);
                default:
                    throw new Exception($"Relationship type for association [{Model.Name}.{associationEnd.Name}] could not be determined.");
            }
        }

        private IEnumerable<CSharpStatement> GetTypeConfiguration(IElement targetType, CSharpClass @class)
        {
            var statements = new List<CSharpStatement>();

            if (targetType.IsClassModel())
                statements.Add(GetKeyMapping(targetType.AsClassModel()));

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
            if (model.HasTable())
            {
                yield return $@"builder.ToTable(""{model.GetTable()?.Name() ?? model.Name}""{(!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()) ? @$", ""{model.GetTable().Schema() ?? "dbo"}""" : "")});";
            }
            else if (ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPH() && model.ParentClass != null)
            {
                yield return $@"builder.HasBaseType<{GetTypeName("Domain.Entity", model.ParentClass)}>();";
            }
            else if (ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPT())
            {
                yield return $@"builder.ToTable(""{model.Name}"");";
            }
            else if (ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC() && !model.IsAbstract)
            {
                yield return $@"builder.ToTable(""{model.Name}"");";
            }
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

        public CSharpStatement GetKeyMapping(ClassModel model)
        {
            if (model.ParentClass != null && (!model.ParentClass.IsAbstract ||
                                              !ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC()))
            {
                return null;
            }

            if (!model.GetExplicitPrimaryKey().Any())
            {
                var rootEntity = model;
                while (rootEntity.ParentClass != null)
                {
                    rootEntity = rootEntity.ParentClass;
                }

                if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, rootEntity.InternalElement, out var template))
                {
                    template.CSharpFile.AfterBuild(file =>
                    {
                        var @class = file.Classes.First();
                        @class.InsertProperty(0, template.UseType(GetDefaultSurrogateKeyType()), "Id", property =>
                        {
                            @class.AddMetadata("primary-keys", new[] { property });
                        });
                    }, int.MinValue);
                }

                return GetKeyMappingStatement("Id");
            }
            else
            {
                if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, model, out var template))
                {
                    template.CSharpFile.AfterBuild(file =>
                    {
                        var @class = file.Classes.First();
                        @class.AddMetadata("primary-keys", @class.GetAllProperties()
                            .Where(x => x.TryGetMetadata<AttributeModel>("model", out var attribute) && attribute.HasPrimaryKey())
                            .ToArray());
                    }, int.MinValue);
                }

                return GetKeyMappingStatement(model.GetExplicitPrimaryKey().Select(x => x.Name.ToPascalCase()).ToArray());
            }
        }

        protected CSharpStatement GetKeyMappingStatement(params string[] keyColumns)
        {
            var keys = keyColumns.Count() == 1
                ? "x." + keyColumns[0]
                : $"new {{ {string.Join(", ", keyColumns.Select(key => $"x.{key}"))} }}";

            return $@"builder.HasKey(x => {keys});";
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

        public void EnsureColumnsOnEntity(ICanBeReferencedType entityModel, params RequiredColumn[] columns)
        {
            if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, entityModel.Id, out var template))
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var associatedClass = file.Classes.First();
                    foreach (var column in columns)
                    {
                        if (!associatedClass.GetAllProperties().Any(x => x.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var associationProperty = associatedClass.Properties.SingleOrDefault(x => x.Name.Equals(column.Name.RemoveSuffix("Id")));

                            if (associationProperty != null)
                            {
                                associatedClass.InsertProperty(associatedClass.Properties.IndexOf(associationProperty), template.UseType(column.Type), column.Name, column.ConfigureProperty);
                            }
                            else
                            {
                                associatedClass.AddProperty(template.UseType(column.Type), column.Name, column.ConfigureProperty);
                            }
                        }
                    }
                }, int.MinValue);
            }
        }

        private bool IsOwned(ICanBeReferencedType type)
        {
            return type.IsOwned(ExecutionContext);
        }

        private IEnumerable<AttributeModel> GetAttributes(IElement model)
        {
            var attributes = new List<AttributeModel>();
            var @class = model.AsClassModel();
            if (@class?.ParentClass != null &&
                @class.ParentClass.IsAbstract &&
                ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC())
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
            if (@class?.ParentClass != null &&
                @class.ParentClass.IsAbstract &&
                ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC())
            {
                associations.AddRange(GetAssociations(@class.ParentClass.InternalElement));
            }

            associations.AddRange(model.AssociatedElements
                .Where(x => x.IsAssociationEndModel() && RequiresConfiguration(x.AsAssociationEndModel()))
                .Select(x => x.AsAssociationEndModel())
                .ToList());
            return associations;
        }

        public record RequiredColumn(string Type, string Name, Action<CSharpProperty> ConfigureProperty = null);
    }

    public abstract class EFCoreConfigStatementBase : CSharpStatement
    {
        protected EFCoreConfigStatementBase() : base(null)
        {
        }
    }
}