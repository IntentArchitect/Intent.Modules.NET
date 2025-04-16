using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.SqlDatabaseProject.Templates.Table
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TableTemplate : IntentTemplateBase<ClassModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.SqlDatabaseProject.TableTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TableTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "sql",
                relativeLocation: GetLocation()
            );
        }

        private string GetLocation()
        {
            var schema = Model.GetSchema()?.Name() ?? "dbo";
            return Path.Combine(schema, "Tables");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var columns = Model.Attributes.Select(attribute =>
            {
                var sqlType = ConvertToSqlType(attribute);
                var constraints = GetColumnConstraints(attribute);
                return $"[{attribute.Name}] {sqlType} {constraints}";
            }).ToList();

            var tableName = Model.GetTable()?.Name() ?? Model.Name;
            var schema = Model.GetSchema()?.Name();
            var fullTableName = string.IsNullOrEmpty(schema) ? $"[{tableName}]" : $"[{schema}].[{tableName}]";

            var primaryKeyAttributes = Model.Attributes.Where(attr => attr.HasPrimaryKey()).ToList();
            var primaryKeyConstraint = primaryKeyAttributes.Any()
                ? $",\n    CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED ({string.Join(", ", primaryKeyAttributes.Select(attr => $"[{attr.Name}] ASC"))})"
                : "";

            var foreignKeyConstraints = Model.Attributes
                .Where(p => p.HasForeignKey())
                .Select(attr =>
                {
                    var foreignKey = attr.GetForeignKey();
                    var otherClass = foreignKey.Association().TypeReference.Element.AsClassModel();
                    var referencedTable = otherClass.Name;
                    var referencedSchema = otherClass.GetSchema()?.Name() ?? "dbo";
                    var fkColumnName = otherClass.GetExplicitPrimaryKey().First().Name;
                    return $",\n    CONSTRAINT [FK_{tableName}_{referencedTable}] FOREIGN KEY ([{attr.Name}]) REFERENCES [{referencedSchema}].[{referencedTable}] ([{fkColumnName}])";
                })
                .ToList();

            var checkConstraints = Model.GetCheckConstraint()?.SQL();
            var checkConstraintClause = string.IsNullOrEmpty(checkConstraints) ? "" : $",\n    CHECK ({checkConstraints})";

            return $"""
                    CREATE TABLE {fullTableName}
                    (
                        {string.Join(",\n    ", columns)}{primaryKeyConstraint}{string.Join("", foreignKeyConstraints)}{checkConstraintClause}
                    );
                    """;
        }

        private static string ConvertToSqlType(AttributeModel attribute)
        {
            var column = attribute.GetColumn();
            if (column != null && !string.IsNullOrWhiteSpace(column.Type()))
            {
                return column.Type();
            }
            var sb = new StringBuilder();

            if (attribute.TypeReference.HasStringType())
                sb.Append("NVARCHAR");
            else if (attribute.TypeReference.HasIntType())
                sb.Append("INT");
            else if (attribute.TypeReference.HasLongType())
                sb.Append("BIGINT");
            else if (attribute.TypeReference.HasDateTimeType())
                sb.Append("DATETIME");
            else if (attribute.TypeReference.HasGuidType())
                sb.Append("UNIQUEIDENTIFIER");
            else if (attribute.TypeReference.HasBoolType())
                sb.Append("BIT");
            else if (attribute.TypeReference.HasDecimalType())
                sb.Append("DECIMAL");
            else if (attribute.TypeReference.HasDateType())
                sb.Append("DATE");


            var textConstraints = attribute.GetTextConstraints();
            if (textConstraints != null)
            {
                if (textConstraints.MaxLength().HasValue)
                {
                    sb.Append($"({textConstraints.MaxLength()})");
                }
                else
                {
                    sb.Append("(MAX)");
                }
            }

            if (sb.Length == 0)
            {
                throw new NotSupportedException("Could not convert attribute type to SQL Type");
            }

            return sb.ToString();
        }

        private static string GetColumnConstraints(AttributeModel attribute)
        {
            var constraints = new List<string>();
            var primaryKey = attribute.GetPrimaryKey();
            if (primaryKey != null && primaryKey.Identity())
            {
                constraints.Add("IDENTITY (1,1)");
            }

            if (attribute.TypeReference.IsNullable)
            {
                constraints.Add("NULL");
            }
            else
            {
                constraints.Add("NOT NULL");
            }

            var defaultConstraint = attribute.GetDefaultConstraint();
            if (defaultConstraint != null)
            {
                constraints.Add($"DEFAULT {defaultConstraint.Value()}");
            }

            // Add more constraints as needed
            return string.Join(" ", constraints);
        }
    }
}
