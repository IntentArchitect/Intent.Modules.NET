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
        private const string DefaultSchema = "dbo";

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
            var schema = GetSchemaName();
            return Path.Combine(schema, "Tables");
        }

        private string GetSchemaName()
        {
            return Model.InternalElement.FindSchema() ?? DefaultSchema;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var tableName = GetTableName();
            var fullTableName = GetFullTableName(tableName);
            
            var columnsDefinition = GenerateColumnsDefinition();
            var primaryKeyConstraint = GeneratePrimaryKeyConstraint(tableName);
            var foreignKeyConstraints = GenerateForeignKeyConstraints(tableName);
            var checkConstraintClause = GenerateCheckConstraint();

            return $"""
                    CREATE TABLE {fullTableName}
                    (
                        {columnsDefinition}{primaryKeyConstraint}{foreignKeyConstraints}{checkConstraintClause}
                    );
                    """;
        }

        private string GetTableName()
        {
            return Model.GetTable()?.Name() ?? Model.Name;
        }

        private string GetFullTableName(string tableName)
        {
            var schema = GetSchemaName();
            return string.IsNullOrEmpty(schema) 
                ? $"[{tableName}]" 
                : $"[{schema}].[{tableName}]";
        }

        private string GenerateColumnsDefinition()
        {
            var columns = Model.Attributes.Select(attribute =>
            {
                var computed = attribute.GetComputedValue();
                if (computed != null)
                {
                    var persist = computed.Stored() ? " PERSISTED" : "";
                    return $"[{attribute.Name}] AS ({computed.SQL()}){persist}";
                }
                else
                {
                    var sqlType = ConvertToSqlType(attribute);
                    var constraints = GetColumnConstraints(attribute);
                    return $"[{attribute.Name}] {sqlType} {constraints}";
                }
            }).ToList();

            return string.Join(",\n    ", columns);
        }

        private string GeneratePrimaryKeyConstraint(string tableName)
        {
            var primaryKeyAttributes = Model.Attributes.Where(attr => attr.HasPrimaryKey()).ToList();
            if (!primaryKeyAttributes.Any())
            {
                return string.Empty;
            }

            var primaryKeyColumns = string.Join(", ", primaryKeyAttributes.Select(attr => $"[{attr.Name}] ASC"));
            return $",\n    CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED ({primaryKeyColumns})";
        }

        private string GenerateForeignKeyConstraints(string tableName)
        {
            var foreignKeyConstraints = Model.Attributes
                .Where(p => p.HasForeignKey())
                .Select(attr => GenerateForeignKeyConstraint(attr, tableName))
                .ToList();

            return string.Join("", foreignKeyConstraints);
        }

        private string GenerateForeignKeyConstraint(AttributeModel attribute, string tableName)
        {
            var foreignKey = attribute.GetForeignKey();
            var otherClass = foreignKey.Association().TypeReference.Element.AsClassModel();
            var referencedTable = otherClass.Name;
            var referencedSchema = otherClass.InternalElement.FindSchema() ?? DefaultSchema;
            var fkColumnName = otherClass.GetExplicitPrimaryKey().First().Name;
            
            return $",\n    CONSTRAINT [FK_{tableName}_{referencedTable}] FOREIGN KEY ([{attribute.Name}]) " +
                   $"REFERENCES [{referencedSchema}].[{referencedTable}] ([{fkColumnName}])";
        }

        private string GenerateCheckConstraint()
        {
            var checkConstraints = Model.GetCheckConstraint()?.SQL();
            return string.IsNullOrEmpty(checkConstraints) 
                ? string.Empty 
                : $",\n    CHECK ({checkConstraints})";
        }

        private static string ConvertToSqlType(AttributeModel attribute)
        {
            var column = attribute.GetColumn();
            if (column != null && !string.IsNullOrWhiteSpace(column.Type()))
            {
                return column.Type();
            }

            var sb = new StringBuilder();

            if (!attribute.TypeReference.HasStringType() && SqlHelper.TryGetSqlType(attribute.TypeReference, out var sqlType))
            {
                sb.Append(sqlType);
            }
            else if (attribute.TypeReference.HasStringType())
            {
                ApplyTextConstraints(attribute, sb);
            }
            else
            {
                throw new NotSupportedException($"Could not convert attribute type '{attribute.TypeReference.Element?.Name}' to SQL Type");
            }

            return sb.ToString();
        }

        private static void ApplyTextConstraints(AttributeModel attribute, StringBuilder sb)
        {
            var textConstraints = attribute.GetTextConstraints();
            if (textConstraints == null)
            {
                return;
            }

            sb.Append(textConstraints.SQLDataType().AsEnum() switch
            {
                AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.DEFAULT => $"NVARCHAR({GetConstraintLength()})",
                AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NTEXT => "NTEXT",
                AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NVARCHAR => $"NVARCHAR({GetConstraintLength()})",
                AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.TEXT => "TEXT",
                AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.VARCHAR => $"VARCHAR({GetConstraintLength()})",
                var dt => throw new ArgumentOutOfRangeException($"Unsupported SQL Data Type: {dt}")
            });

            return;
            string GetConstraintLength()
            {
                return textConstraints.MaxLength().HasValue 
                    ? textConstraints.MaxLength()!.Value.ToString() 
                    : "MAX";
            }
        }

        private static string GetColumnConstraints(AttributeModel attribute)
        {
            var constraints = new List<string>();
            
            // Add identity constraint for auto-incrementing primary keys
            AddIdentityConstraint(attribute, constraints);
            
            // Add nullability constraint
            AddNullabilityConstraint(attribute, constraints);
            
            // Add default value constraint
            AddDefaultValueConstraint(attribute, constraints);

            return string.Join(" ", constraints);
        }

        private static void AddIdentityConstraint(AttributeModel attribute, List<string> constraints)
        {
            var primaryKey = attribute.GetPrimaryKey();
            if (primaryKey != null && (primaryKey.DataSource().IsDefault() || primaryKey.DataSource().IsAutoGenerated()))
            {
                constraints.Add("IDENTITY (1,1)");
            }
        }

        private static void AddNullabilityConstraint(AttributeModel attribute, List<string> constraints)
        {
            constraints.Add(attribute.TypeReference.IsNullable ? "NULL" : "NOT NULL");
        }

        private static void AddDefaultValueConstraint(AttributeModel attribute, List<string> constraints)
        {
            var defaultConstraint = attribute.GetDefaultConstraint();
            if (defaultConstraint != null)
            {
                constraints.Add($"DEFAULT {defaultConstraint.Value()}");
            }
        }
    }
}
