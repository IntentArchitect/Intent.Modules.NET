using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class EntityTypeConfigurationTemplate : CSharpTemplateBase<ClassModel, EntityTypeConfigurationDecorator>
    {
        private readonly List<AttributeModel> _explicitPrimaryKeys;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.EntityTypeConfiguration";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public EntityTypeConfigurationTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            _explicitPrimaryKeys = Model.Attributes.Where(x => x.HasPrimaryKey()).ToList();
            AddNugetDependency(NugetPackages.EntityFrameworkCore(Project));
            AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(Project));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Configuration",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetEntityName()
        {
            return GetTypeName(DomainEntityStateTemplate.TemplateId, Model);
        }

        private string GetTableMapping()
        {
            if (Model.ParentClass != null)
            {
                return $@"
            builder.HasBaseType<{ Model.ParentClass.Name }>();
";

            }
            if (Model.HasTable())
            {
                return $@"
            builder.ToTable(""{ Model.GetTable()?.Name() ?? Model.Name }"", ""{ Model.GetTable()?.Schema() ?? "dbo" }"");
";
            }

            return string.Empty;
        }

        private string GetKeyMapping()
        {
            if (Model.ParentClass != null)
            {
                return string.Empty;
            }
            if (!_explicitPrimaryKeys.Any())
            {
                return $@"
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .UsePropertyAccessMode(PropertyAccessMode.Property)
                   .ValueGeneratedNever();";
            }
            else
            {
                var keys = _explicitPrimaryKeys.Count() == 1
                    ? "x." + _explicitPrimaryKeys.Single().Name.ToPascalCase()
                    : $"new {{ {string.Join(", ", _explicitPrimaryKeys.Select(x => "x." + x.Name))} }}";
                return $@"
            builder.HasKey(x => {keys});";
            }
        }

        private bool HasMapping(AttributeModel attribute)
        {
            return _explicitPrimaryKeys.Any() || !attribute.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool HasMapping(AssociationEndModel associationEnd)
        {
            return associationEnd.IsTargetEnd();
        }

        private string GetAttributeMapping(AttributeModel attribute)
        {
            var statements = new List<string>();
            statements.Add($"builder.Property(x => x.{ attribute.Name.ToPascalCase() })");
            if (!attribute.Type.IsNullable)
            {
                statements.Add(".IsRequired()");
            }

            if (attribute.GetPrimaryKey()?.Identity() == true)
            {
                statements.Add(".UseSqlServerIdentityColumn()");
            }

            if (attribute.GetDefaultConstraint()?.Value() != null)
            {
                var defaultValue = attribute.GetDefaultConstraint().Value();
                statements.Add($".HasDefaultValueSql({(attribute.Type.Element.Name == "string" ? $"\"{ defaultValue }\"" : defaultValue)})");
            }

            var maxLength = attribute.GetTextConstraints()?.MaxLength();
            if (maxLength.HasValue && attribute.Type.Element.Name == "string")
            {
                statements.Add($".HasMaxLength({maxLength.Value})");
            }

            var decimalPrecision = attribute.GetDecimalConstraints()?.Precision();
            var decimalScale = attribute.GetDecimalConstraints()?.Scale();
            if (decimalPrecision.HasValue && decimalScale.HasValue)
            {
                statements.Add($".HasColumnType(\"decimal({ decimalPrecision }, { decimalScale })\")");
            }

            return $@"
            {string.Join(@"
                ", statements)};";
        }

        private string GetAssociationMapping(AssociationEndModel associationEnd)
        {
            var statements = new List<string>();

            switch (associationEnd.Association.GetRelationshipType())
            {
                case RelationshipType.OneToOne:
                    statements.Add($"builder.HasOne(x => x.{associationEnd.Name().ToPascalCase()})");
                    statements.Add($".WithOne({ (associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name().ToPascalCase()}" : "") })");
                    statements.Add($".HasForeignKey<{ Model.Name }>({GetForeignKeyLambda(associationEnd.OtherEnd())})");
                    if (!associationEnd.OtherEnd().IsNullable)
                    {
                        statements.Add($".IsRequired()");
                        statements.Add($".OnDelete(DeleteBehavior.Cascade)");
                    }
                    else
                    {
                        statements.Add($".OnDelete(DeleteBehavior.Restrict)");
                    }

                    break;
                case RelationshipType.ManyToOne:
                    statements.Add($"builder.HasOne(x => x.{associationEnd.Name().ToPascalCase()})");
                    statements.Add($".WithMany({ (associationEnd.OtherEnd().IsNavigable ? "x => x." + associationEnd.OtherEnd().Name().ToPascalCase() : "") })");
                    statements.Add($".HasForeignKey({GetForeignKeyLambda(associationEnd.OtherEnd())})");
                    statements.Add($".OnDelete(DeleteBehavior.Restrict)");
                    break;
                case RelationshipType.OneToMany:
                    statements.Add($"builder.HasMany(x => x.{associationEnd.Name().ToPascalCase()})");
                    statements.Add($".WithOne(x => x.{ associationEnd.OtherEnd().Name().ToPascalCase() })");
                    statements.Add($".HasForeignKey({GetForeignKeyLambda(associationEnd)})");
                    if (!associationEnd.OtherEnd().IsNullable)
                    {
                        statements.Add($".IsRequired()");
                        statements.Add($".OnDelete(DeleteBehavior.Cascade)");
                    }

                    break;
                case RelationshipType.ManyToMany:
                    if (Project.GetProject().IsNetCore2App || Project.GetProject().IsNetCore3App)
                    {
                        statements.Add($"builder.Ignore(x => x.{associationEnd.Name().ToPascalCase()})");

                        Logging.Log.Warning($@"Intent.EntityFrameworkCore: Cannot create mapping relationship from {Model.Name} to {associationEnd.Class.Name}. It has been ignored, and will not be persisted.
    Many-to-Many relationships are not yet supported by EntityFrameworkCore as yet.
    Either upgrade your solution to .NET 5.0 or create a joining-table entity (e.g. [{Model.Name}] 1 --> * [{Model.Name}{associationEnd.Class.Name}] * --> 1 [{associationEnd.Class.Name}])
    For more information, please see https://github.com/aspnet/EntityFrameworkCore/issues/1368");
                    }
                    else // if .NET 5.0 or above...
                    {
                        statements.Add($"builder.HasMany(x => x.{associationEnd.Name().ToPascalCase()})");
                        statements.Add($".WithMany(x => x.{associationEnd.OtherEnd().Name().ToPascalCase()})");
                        statements.Add($".UsingEntity(x => x.ToTable(\"{associationEnd.OtherEnd().Class.Name}{associationEnd.Class.Name.ToPluralName()}\"))");
                    }
                    break;
                default:
                    throw new Exception($"Relationship type for association [{Model.Name}.{associationEnd.Name}] could not be determined.");
            }
            return $@"
            {string.Join(@"
                ", statements)};";
        }

        private string GetIndexes()
        {
            var indexes = Model.Attributes
                .Where(x => x.HasIndex())
                .GroupBy(x => x.GetIndex().UniqueKey() ?? "IX_" + Model.Name + "_" + x.Name);

            var statements = new List<string>();
            foreach (var index in indexes)
            {
                var indexFields = index.Count() == 1
                    ? "x." + index.Single().Name.ToPascalCase()
                    : $"new {{ {string.Join(", ", index.OrderBy(x => x.GetIndex().Order() ?? 0).Select(x => "x." + x.Name))} }}";
                statements.Add($@"
            builder.HasIndex(x => {indexFields})
                .HasName(""{ index.Key }"")
                .IsUnique({ index.First().GetIndex().IsUnique().ToString().ToLower() ?? "false" });");
            }
            return statements.Any() ? $@"
{string.Join(@"
", statements)}" : string.Empty;
        }

        private string GetForeignKeyLambda(AssociationEndModel associationEnd)
        {
            var columns = ((associationEnd.IsSourceEnd()
                               ? (associationEnd as AssociationSourceEndModel).GetForeignKey()?.ColumnName()
                               : (associationEnd as AssociationTargetEndModel).GetForeignKey()?.ColumnName())
                           ?? associationEnd.OtherEnd().Name().ToPascalCase() + "Id")
                .Split(',') // upgrade stereotype to have a selection list.
                .Select(x => x.Trim())
                .ToList();

            if (columns.Count == 1)
            {
                return $"x => x.{columns.Single()}";
            }

            return $"x => new {{ {string.Join(", ", columns.Select(x => "x." + x))}}}";
        }
    }
}