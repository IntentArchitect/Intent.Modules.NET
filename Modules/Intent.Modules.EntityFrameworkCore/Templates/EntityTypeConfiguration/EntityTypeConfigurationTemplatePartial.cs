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
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Api.Indexes;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AttributeModelStereotypeExtensions = Intent.Metadata.RDBMS.Api.AttributeModelStereotypeExtensions;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    public class EntityTypeConfigurationCreatedEvent
    {
        public EntityTypeConfigurationCreatedEvent(EntityTypeConfigurationTemplate template)
        {
            Template = template;
        }

        public EntityTypeConfigurationTemplate Template { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class EntityTypeConfigurationTemplate : CSharpTemplateBase<ClassModel, EntityTypeConfigurationDecorator>, ICSharpFileBuilderTemplate
    {
        //private readonly List<AttributeModel> _explicitPrimaryKeys;
        private readonly List<Action<CSharpClass>> _requiredChangesToDomainEntity = new List<Action<CSharpClass>>();

        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.EntityFrameworkCore.EntityTypeConfiguration";

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
                    var entityTemplate = GetTemplate<IIntentTemplate>("Domain.Entity", Model);
                    @class.ImplementsInterface($"IEntityTypeConfiguration<{GetTypeName(entityTemplate)}>")
                        .AddMethod("void", "Configure", method =>
                        {
                            method.AddParameter($"EntityTypeBuilder<{GetTypeName(entityTemplate)}>", "builder");
                            method.AddStatements(new[] {
                                GetTableMapping(Model),
                                GetKeyMapping(Model),
                                GetCheckConstraints(),
                                GetBeforeAttributeStatements()
                            }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

                            if (entityTemplate is ICSharpFileBuilderTemplate builderTemplate)
                            {
                                builderTemplate.CSharpFile.OnBuild(file =>
                                {
                                    foreach (var requiredChange in _requiredChangesToDomainEntity)
                                    {
                                        requiredChange.Invoke(file.Classes.First());
                                    }
                                    foreach (var property in file.Classes.First().Properties.ToList())
                                    {
                                        if (property.TryGetMetadata<AttributeModel>("model", out var attribute))
                                        {
                                            method.AddStatement(GetAttributeMapping(attribute, @class));
                                        }
                                        else if (property.TryGetMetadata<AssociationEndModel>("model", out var associationEnd))
                                        {
                                            method.AddStatement(GetAssociationMapping(associationEnd, @class));
                                        }
                                        if (property.TryGetMetadata<bool>("non-persistent", out var nonPersistent) && nonPersistent)
                                        {
                                            method.AddStatement($"builder.Ignore(e => e.{property.Name});");
                                        }
                                    }
                                    method.Statements.SeparateAll();
                                }, 100);
                            }
                            else
                            {
                                // Backward compatibility
                                foreach (var attribute in GetAttributes(Model))
                                {
                                    method.AddStatement(GetAttributeMapping(attribute, @class));
                                }
                                foreach (var associationEnd in GetAssociations(Model))
                                {
                                    method.AddStatement(GetAssociationMapping(associationEnd, @class));
                                }
                            }

                            method.AddStatements(GetIndexes(Model));
                            method.Statements.SeparateAll();
                        });
                });
        }
        public CSharpFile CSharpFile { get; }

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

        private string GetTableMapping(ClassModel model)
        {
            if (model.ParentClass != null && ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy()
                    .IsTPH())
            {
                return $@"
            builder.HasBaseType<{GetTypeName("Domain.Entity", model.ParentClass)}>();
";
            }

            if (model.HasTable() || !ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy()
                    .IsTPH())
            {
                return $@"
            builder.ToTable(""{model.GetTable()?.Name() ?? model.Name}""{(!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()) ? @$", ""{model.GetTable().Schema() ?? "dbo"}""" : "")});
";
            }

            if (ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos() && model.IsAggregateRoot())
            {
                // Is there an easier way to get this?
                var domainPackage = new DomainPackageModel(this.Model.InternalElement.Package);
                var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

                var containerName = string.IsNullOrWhiteSpace(cosmosSettings?.ContainerName())
                    ? OutputTarget.ApplicationName()
                    : cosmosSettings.ContainerName();

                return $@"
            builder.ToContainer(""{containerName}"");
";
            }

            return string.Empty;
        }

        private string GetKeyMapping(ClassModel model)
        {
            if (model.ParentClass != null && (!model.ParentClass.IsAbstract || !ExecutionContext.Settings
                    .GetDatabaseSettings().InheritanceStrategy().IsTPC()))
            {
                return string.Empty;
            }

            if (!model.GetExplicitPrimaryKey().Any())
            {
                if (IsOwned(model.InternalElement))
                {
                    return "";
                }

                _requiredChangesToDomainEntity.Add((@class) =>
                {
                    @class.InsertProperty(0, this.GetDefaultSurrogateKeyType(), "Id");
                });

                return $@"builder.HasKey(x => x.Id);";
            }
            else
            {
                var keys = model.GetExplicitPrimaryKey().Count() == 1
                    ? "x." + model.GetExplicitPrimaryKey().Single().Name.ToPascalCase()
                    : $"new {{ {string.Join(", ", model.GetExplicitPrimaryKey().Select(x => "x." + x.Name.ToPascalCase()))} }}";
                return $@"builder.HasKey(x => {keys});";
            }
        }

        private bool RequiresConfiguration(AttributeModel attribute)
        {
            return Model.GetExplicitPrimaryKey().All(key => !key.Equals(attribute)) &&
                   !attribute.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool RequiresConfiguration(AssociationEndModel associationEnd)
        {
            return associationEnd.IsTargetEnd();
        }

        private string GetAttributeMapping(AttributeModel attribute, CSharpClass @class)
        {
            var statements = new List<string>();

            if (!IsOwned(attribute.TypeReference.Element))
            {
                statements.Add($"builder.Property(x => x.{attribute.Name.ToPascalCase()})");
                statements.AddRange(GetAttributeMappingStatements(attribute));

                return $@"{string.Join(@"
                ", statements)};";
            }

            @class.AddMethod("void", $"Configure{attribute.Name.ToPascalCase()}", method =>
            {
                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(attribute.InternalElement.ParentElement)}, {GetTypeName((IElement)attribute.TypeReference.Element)}>", "builder");
                method.AddStatements(GetTypeConfiguration((IElement)attribute.TypeReference.Element, @class).ToArray());
                method.Statements.SeparateAll();
            });

            if (attribute.TypeReference.IsCollection)
            {
                statements.Add($"builder.OwnsMany(x => x.{attribute.Name.ToPascalCase()}, Configure{attribute.Name.ToPascalCase()})");
            }
            else
            {
                statements.Add($"builder.OwnsOne(x => x.{attribute.Name.ToPascalCase()}, Configure{attribute.Name.ToPascalCase()})");
                if (!attribute.TypeReference.IsNullable)
                {
                    statements.Add($".Navigation(x => x.{attribute.Name.ToPascalCase()}).IsRequired()");
                }
            }

            return $@"{string.Join(@"
                ", statements)};";
        }

        private List<string> GetAttributeMappingStatements(AttributeModel attribute)
        {
            var statements = new List<string>();
            if (!attribute.Type.IsNullable)
            {
                statements.Add(".IsRequired()");
            }

            if (attribute.GetPrimaryKey()?.Identity() == true)
            {
                statements.Add(".UseSqlServerIdentityColumn()");
            }

            if (attribute.HasDefaultConstraint())
            {
                var treatAsSqlExpression = attribute.GetDefaultConstraint().TreatAsSQLExpression();
                var defaultValue = attribute.GetDefaultConstraint()?.Value() ?? string.Empty;

                if (!treatAsSqlExpression &&
                    !defaultValue.TrimStart().StartsWith("\"") &&
                    attribute.Type.Element.Name == "string")
                {
                    defaultValue = $"\"{defaultValue}\"";
                }

                var method = treatAsSqlExpression
                    ? "HasDefaultValueSql"
                    : "HasDefaultValue";

                statements.Add($".{method}({defaultValue})");
            }

            if (attribute.GetTextConstraints()?.SQLDataType().IsDEFAULT() == true)
            {
                var maxLength = attribute.GetTextConstraints().MaxLength();
                if (maxLength.HasValue && attribute.Type.Element.Name == "string")
                {
                    statements.Add($".HasMaxLength({maxLength.Value})");
                }

                //var isUnicode = attribute.GetTextConstraints().SQLDataType().IsVARCHAR() == true;
                //if (isUnicode)
                //{
                //    statements.Add($".IsUnicode(false)");
                //}
            }
            else if (attribute.HasTextConstraints())
            {
                var maxLength = attribute.GetTextConstraints().MaxLength();
                switch (attribute.GetTextConstraints().SQLDataType().AsEnum())
                {
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.VARCHAR:
                        statements.Add($".HasColumnType(\"varchar({maxLength?.ToString() ?? "max"})\")");
                        break;
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NVARCHAR:
                        statements.Add($".HasColumnType(\"nvarchar({maxLength?.ToString() ?? "max"})\")");
                        break;
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.TEXT:
                        Logging.Log.Warning(
                            $"{Model.Name}.{attribute.Name}: The ntext, text, and image data types will be removed in a future version of SQL Server. Avoid using these data types in new development work, and plan to modify applications that currently use them. Use nvarchar(max), varchar(max), and varbinary(max) instead.");
                        statements.Add($".HasColumnType(\"text\")");
                        break;
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NTEXT:
                        Logging.Log.Warning(
                            $"{Model.Name}.{attribute.Name}: The ntext, text, and image data types will be removed in a future version of SQL Server. Avoid using these data types in new development work, and plan to modify applications that currently use them. Use nvarchar(max), varchar(max), and varbinary(max) instead.");
                        statements.Add($".HasColumnType(\"ntext\")");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                var decimalPrecision = attribute.GetDecimalConstraints()?.Precision();
                var decimalScale = attribute.GetDecimalConstraints()?.Scale();
                var columnType = attribute.GetColumn()?.Type();
                if (decimalPrecision.HasValue && decimalScale.HasValue)
                {
                    statements.Add($".HasColumnType(\"decimal({decimalPrecision}, {decimalScale})\")");
                }
                else if (!string.IsNullOrWhiteSpace(columnType))
                {
                    statements.Add($".HasColumnType(\"{columnType}\")");
                }
            }

            var columnName = attribute.GetColumn()?.Name();
            if (!string.IsNullOrWhiteSpace(columnName))
            {
                statements.Add($".HasColumnName(\"{columnName}\")");
            }

            var computedValueSql = attribute.GetComputedValue()?.SQL();
            if (!string.IsNullOrWhiteSpace(computedValueSql))
            {
                statements.Add(
                    $".HasComputedColumnSql(\"{computedValueSql}\"{(attribute.GetComputedValue().Stored() ? ", stored: true" : string.Empty)})");
            }

            if (Intent.EntityFrameworkCore.Api.AttributeModelStereotypeExtensions.HasRowVersion(attribute))
            {
                statements.Add($".IsRowVersion()");
            }

            return statements;
        }

        private string GetAfterAttributeStatements()
        {
            // These will have their own whitespace padding so we want to try and retain this.
            // Once this can be deprecated, then it will simplify this method a lot.
            string statementsCode = GetDecoratorsOutput(x => x.AfterAttributes() ?? string.Empty);

            var statements = new List<string>();
            statements.AddRange(GetDecorators().SelectMany(x => x.AfterAttributeStatements()));

            if (ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            {
                // Is there an easier way to get this?
                var domainPackage = new DomainPackageModel(this.Model.InternalElement.Package);
                var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

                var partitionKey = cosmosSettings?.PartitionKey()?.ToPascalCase();
                if (string.IsNullOrEmpty(partitionKey))
                {
                    partitionKey = "PartitionKey";
                }
                if (GetAttributes(Model).Any(p => p.Name.ToPascalCase().Equals(partitionKey) && p.HasPartitionKey()))
                {
                    statements.Add($@"builder.HasPartitionKey(x => x.{partitionKey});");
                }
            }

            if (statements.Count > 0)
            {
                const string newLine = @"
            ";
                var joined = string.Join(newLine, statements);

                statementsCode = !string.IsNullOrEmpty(statementsCode)
                    ? statementsCode + newLine + joined
                    : joined;
            }

            return statementsCode;
        }

        private string GetAssociationMapping(AssociationEndModel associationEnd, CSharpClass @class)
        {
            var statements = new List<string>();

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
                        @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                        {
                            method.AddParameter($"OwnedNavigationBuilder<{GetTypeName((IElement)associationEnd.OtherEnd().Element)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                            method.AddStatement($"builder.WithOwner({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : "")}){(!IsValueObject(associationEnd.Element) ? ".HasForeignKey(x => x.Id)" : "")};");
                            method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class).ToArray());
                            method.Statements.SeparateAll();
                        });
                        statements.Add($"builder.OwnsOne(x => x.{associationEnd.Name.ToPascalCase()}, Configure{associationEnd.Name.ToPascalCase()})");

                        if (!associationEnd.IsNullable)
                        {
                            statements.Add($"    .Navigation(x => x.{associationEnd.Name.ToPascalCase()}).IsRequired()");
                        }

                        return $@"{string.Join(@"
            ", statements)};";
                    }

                    statements.Add($"builder.HasOne(x => x.{associationEnd.Name.ToPascalCase()})");
                    statements.Add($"    .WithOne({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : "")})");

                    if (associationEnd.IsNullable)
                    {
                        if (associationEnd.OtherEnd().IsNullable)
                        {
                            statements.Add($"    .HasForeignKey<{associationEnd.OtherEnd().Class.Name}>({GetForeignKeyLambda(associationEnd.OtherEnd(), out var columns)})");
                            EnsureColumnsOnEntity(associationEnd.OtherEnd(), columns);
                        }
                        else
                        {
                            statements.Add($"    .HasForeignKey<{associationEnd.Class.Name}>(x => x.Id)");
                        }
                    }
                    else
                    {
                        statements.Add($"    .HasForeignKey<{Model.Name}>(x => x.Id)");
                    }

                    if (!associationEnd.OtherEnd().IsNullable)
                    {
                        statements.Add($"    .IsRequired()");
                        statements.Add($"    .OnDelete(DeleteBehavior.Cascade)");
                    }
                    else
                    {
                        statements.Add($"    .OnDelete(DeleteBehavior.Restrict)");
                    }

                    break;
                case RelationshipType.ManyToOne:
                    {
                        statements.Add($"builder.HasOne(x => x.{associationEnd.Name.ToPascalCase()})");
                        statements.Add($"    .WithMany({(associationEnd.OtherEnd().IsNavigable ? "x => x." + associationEnd.OtherEnd().Name.ToPascalCase() : "")})");
                        statements.Add($"    .HasForeignKey({GetForeignKeyLambda(associationEnd.OtherEnd(), out var columns)})");
                        statements.Add($"    .OnDelete(DeleteBehavior.Restrict)");
                        EnsureColumnsOnEntity(associationEnd.OtherEnd(), columns);
                        break;
                    }
                case RelationshipType.OneToMany:
                    {
                        if (IsOwned(associationEnd.Element))
                        {
                            @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                            {
                                (string Type, string Name)[] columns = null;
                                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName((IElement)associationEnd.OtherEnd().Element)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                                method.AddStatement($"builder.WithOwner({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : "")}){(!IsValueObject(associationEnd.Element) ? $".HasForeignKey({GetForeignKeyLambda(associationEnd, out columns)})" : "")};");
                                method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class).ToArray());
                                EnsureColumnsOnEntity(associationEnd, columns ?? Array.Empty<(string Type, string Name)>());
                                method.Statements.SeparateAll();
                            });
                            return $@"builder.OwnsMany(x => x.{associationEnd.Name.ToPascalCase()}, Configure{associationEnd.Name.ToPascalCase()});";
                        }

                        statements.Add($"builder.HasMany(x => x.{associationEnd.Name.ToPascalCase()})");
                        statements.Add($"    .WithOne({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : $"")})");
                        statements.Add($"    .HasForeignKey({GetForeignKeyLambda(associationEnd, out var columns)})");
                        EnsureColumnsOnEntity(associationEnd, columns);
                        if (!associationEnd.OtherEnd().IsNullable)
                        {
                            statements.Add($"    .IsRequired()");
                            statements.Add($"    .OnDelete(DeleteBehavior.Cascade)");
                        }

                        break;
                    }
                case RelationshipType.ManyToMany:
                    if (Project.GetProject().IsNetCore2App || Project.GetProject().IsNetCore3App)
                    {
                        statements.Add($"builder.Ignore(x => x.{associationEnd.Name.ToPascalCase()})");

                        Logging.Log.Warning(
                            $@"Intent.EntityFrameworkCore: Cannot create mapping relationship from {Model.Name} to {associationEnd.Class.Name}. It has been ignored, and will not be persisted.
    Many-to-Many relationships are not yet supported by EntityFrameworkCore as yet.
    Either upgrade your solution to .NET 5.0 or create a joining-table entity (e.g. [{Model.Name}] 1 --> * [{Model.Name}{associationEnd.Class.Name}] * --> 1 [{associationEnd.Class.Name}])
    For more information, please see https://github.com/aspnet/EntityFrameworkCore/issues/1368");
                    }
                    else // if .NET 5.0 or above...
                    {
                        statements.Add($"builder.HasMany(x => x.{associationEnd.Name.ToPascalCase()})");
                        statements.Add($"    .WithMany({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : $"\"{associationEnd.OtherEnd().Name.ToPascalCase()}\"")})");
                        statements.Add($"    .UsingEntity(x => x.ToTable(\"{associationEnd.OtherEnd().Class.Name}{associationEnd.Class.Name.ToPluralName()}\"))");
                    }

                    break;
                default:
                    throw new Exception($"Relationship type for association [{Model.Name}.{associationEnd.Name}] could not be determined.");
            }

            return $@"{string.Join(@"
            ", statements)};";
        }

        private void EnsureColumnsOnEntity(AssociationEndModel associationEnd, params (string Type, string Name)[] columns)
        {
            if (TryGetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", associationEnd.Element.Id, out var template))
            {
                var associatedClass = template.CSharpFile.Classes.First();
                foreach (var column in columns)
                {
                    if (!associatedClass.Properties.Any(x => x.Name.Equals(column.Name)))
                    {
                        var associationProperty = associatedClass.Properties.SingleOrDefault(x => x.Name.Equals(column.Name.RemoveSuffix("Id")));
                        if (associationProperty != null)
                        {
                            associatedClass.InsertProperty(associatedClass.Properties.IndexOf(associationProperty), column.Type, column.Name);
                        }
                        else
                        {
                            associatedClass.AddProperty(column.Type, column.Name);
                        }
                    }
                }
            }
        }

        private List<string> GetTypeConfiguration(IElement targetType, CSharpClass @class)
        {
            var statements = new List<string>();
            var attributes = targetType.ChildElements
                .Where(x => x.IsAttributeModel())
                .Select(x => x.AsAttributeModel())
                .ToList();

            var associations = targetType.AssociatedElements
                .Where(x => x.IsAssociationEndModel())
                .Select(x => x.AsAssociationEndModel())
                .ToList();

            if (targetType.IsClassModel())
            {
                statements.Add(GetTableMapping(targetType.AsClassModel()));
                if (targetType.AsClassModel().GetExplicitPrimaryKey().Any())
                {
                    statements.Add(GetKeyMapping(targetType.AsClassModel()));
                }
            }

            statements.AddRange(attributes.Where(RequiresConfiguration).Select(attribute => GetAttributeMapping(attribute, @class)));
            statements.AddRange(associations.Where(RequiresConfiguration).Select(association => GetAssociationMapping(association, @class)));
            if (targetType.IsClassModel())
            {
                statements.AddRange(GetIndexes(targetType.AsClassModel()));
            }

            return statements.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        private string GetCheckConstraints()
        {
            var checkConstraints = Model.GetCheckConstraints();
            if (checkConstraints.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(@"
            builder");

            foreach (var checkConstraint in checkConstraints)
            {
                sb.Append(@$"
                .HasCheckConstraint(""{checkConstraint.Name()}"", ""{checkConstraint.SQL()}"")");
            }

            sb.Append(";");
            return sb.ToString();
        }

        private string[] GetIndexes(ClassModel model)
        {
            var indexes = model.GetIndexes();
            if (indexes.Count == 0)
            {
                return Array.Empty<string>();
            }

            var statements = new List<string>();

            foreach (var index in indexes)
            {
                var indexFields = index.KeyColumns.Length == 1
                    ? GetIndexColumnPropertyName(index.KeyColumns.Single(), "x.")
                    : $"new {{ {string.Join(", ", index.KeyColumns.Select(x => GetIndexColumnPropertyName(x, "x.")))} }}";

                var sb = new StringBuilder($@"
            builder.HasIndex(x => {indexFields})");

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

            return statements.ToArray();
        }

        private bool IsValueObject(ICanBeReferencedType type)
        {
            return TryGetTypeName("Domain.ValueObject", type.Id, out var typename);
        }

        private bool IsOwned(ICanBeReferencedType type)
        {
            if (type.IsClassModel())
            {
                return !type.AsClassModel().IsAggregateRoot();
            }

            return IsValueObject(type);
        }

        private static string GetIndexColumnPropertyName(IndexColumn column, string prefix = null)
        {
            return column.SourceType.IsAssociationEndModel()
                ? $"{prefix}{column.Name.ToPascalCase()}Id"
                : $"{prefix}{column.Name.ToPascalCase()}";
        }

        private string GetForeignKeyLambda(AssociationEndModel associationEnd, out (string Type, string Name)[] columns)
        {
            //if (associationEnd.HasForeignKey() &&
            //    !string.IsNullOrWhiteSpace(associationEnd.GetForeignKey().ColumnName()))
            //{
            //    columns.AddRange(associationEnd.GetForeignKey().ColumnName()
            //        .Split(',') // upgrade stereotype to have a selection list.
            //        .Select(x => x.Trim()));
            //}
            if (associationEnd.OtherEnd().Class.GetExplicitPrimaryKey().Any())
            {
                columns = associationEnd.OtherEnd().Class.GetExplicitPrimaryKey().Select(x =>
                    (Type: GetTypeName(x), Name: $"{associationEnd.OtherEnd().Name.ToPascalCase()}{x.Name.ToPascalCase()}"))
                    .ToArray();
            }
            else // implicit Id
            {
                columns = new[] { (Type: this.GetDefaultSurrogateKeyType(), Name: $"{associationEnd.OtherEnd().Name.ToPascalCase()}Id") };
            }

            if (columns?.Length == 1)
            {
                return $"x => x.{columns.Single().Name}";
            }

            return $"x => new {{ {string.Join(", ", columns.Select(x => "x." + x.Name))}}}";
        }

        private string GetBeforeAttributeStatements()
        {
            var statements = new List<string>();

            statements.AddRange(GetDecorators().SelectMany(x => x.BeforeAttributeStatements()));

            // These will have their own whitespace padding so we want to try and retain this.
            // Once this can be deprecated, then it will simplify this method a lot.
            string statementsCode = GetDecoratorsOutput(x => x.BeforeAttributes() ?? string.Empty);

            if (statements.Count > 0 || !string.IsNullOrEmpty(statementsCode))
            {
                const string newLine = @"
            ";
                var joined = string.Join(newLine, statements);
                return newLine + joined + (!string.IsNullOrWhiteSpace(statementsCode) ? newLine : string.Empty) + statementsCode;
            }

            return string.Empty;
        }

        private IEnumerable<AttributeModel> GetAttributes(ClassModel model)
        {
            var attributes = new List<AttributeModel>();
            if (model.ParentClass != null && model.ParentClass.IsAbstract && ExecutionContext.Settings
                    .GetDatabaseSettings().InheritanceStrategy().IsTPC())
            {
                attributes.AddRange(GetAttributes(model.ParentClass));
            }

            attributes.AddRange(model.Attributes.Where(RequiresConfiguration).ToList());
            return attributes;
        }

        private IEnumerable<AssociationEndModel> GetAssociations(ClassModel model)
        {
            var associations = new List<AssociationEndModel>();
            if (model.ParentClass != null && model.ParentClass.IsAbstract && ExecutionContext.Settings
                    .GetDatabaseSettings().InheritanceStrategy().IsTPC())
            {
                associations.AddRange(GetAssociations(model.ParentClass));
            }

            associations.AddRange(model.AssociatedClasses.Where(RequiresConfiguration).ToList());
            return associations;
        }
    }
}