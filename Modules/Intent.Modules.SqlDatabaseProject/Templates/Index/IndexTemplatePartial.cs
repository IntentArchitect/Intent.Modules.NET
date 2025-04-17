using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using IndexModel = Intent.SqlDatabaseProject.Api.IndexModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.SqlDatabaseProject.Templates.Index
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class IndexTemplate : IntentTemplateBase<IndexModel>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.SqlDatabaseProject.IndexTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public IndexTemplate(IOutputTarget outputTarget, IndexModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Index.Name}",
                fileExtension: "sql",
                relativeLocation: GetLocation()
            );
        }

        private string GetLocation()
        {
            var schema = GetSchemaName();
            return Path.Combine(schema, "Indexes");
        }

        private string GetSchemaName()
        {
            return Model.ClassModel.GetSchema()?.Name() ?? "dbo";
        }

        private string GetTableName()
        {
            return Model.ClassModel.GetTable()?.Name() ?? Model.ClassModel.Name;
        }

        private string GetFullTableName(string tableName)
        {
            var schema = GetSchemaName();
            return string.IsNullOrEmpty(schema)
                ? $"[{tableName}]"
                : $"[{schema}].[{tableName}]";
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var tableName = GetTableName();
            var fullTableName = GetFullTableName(tableName);

            var sb = new StringBuilder();
            sb.Append("CREATE ");
            
            if (Model.Index.IsUnique)
            {
                sb.Append("UNIQUE ");
            }

            sb.Append("INDEX ");
            sb.Append('[');
            sb.Append(Model.Index.Name);
            sb.Append(']');
            sb.AppendLine();
            
            sb.Append(" ON ");
            sb.Append(fullTableName);
            sb.Append('(');
            sb.Append(string.Join(", ", Model.Index.KeyColumns.Select(x => x.Name)));
            sb.Append(')');
            sb.AppendLine();

            if (Model.Index.IncludedColumns.Length != 0)
            {
                sb.Append("INCLUDE (");
                sb.Append(string.Join(", ", Model.Index.IncludedColumns.Select(x => x.Name)));
                sb.Append(')');
            }
            
            return sb.ToString();
        }
    }
}