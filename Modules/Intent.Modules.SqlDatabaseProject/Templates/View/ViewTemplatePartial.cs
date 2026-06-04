using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Intent.Modules.SqlDatabaseProject.Templates.View
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ViewTemplate : IntentTemplateBase<ClassModel>
    {
        private const string DefaultSchema = "dbo";
        private const string ViewFolder = "Views";
        private const string ViewStereotypeName = "View";
        private const string ViewNameProperty = "Name";

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.SqlDatabaseProject.ViewTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ViewTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: GetViewName(),
                fileExtension: "sql",
                relativeLocation: GetLocation(),
                OverwriteBehaviour.OverwriteDisabled
            );
        }

        private string GetLocation()
        {
            return Path.Combine(GetSchemaName(), ViewFolder);
        }

        private string GetSchemaName()
        {
            return Model.InternalElement.FindSchema() ?? DefaultSchema;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $"""
                    CREATE VIEW {GetFullViewName()}
                    AS
                    SELECT
                        -- TODO: Add columns and source query
                    {GenerateCommentedExampleSelect()}
                    ;
                    """;
        }

        private string GetViewName()
        {
            var configuredName = Model.GetView()?.Name()?.Trim()
                                 ?? Model.GetStereotypeProperty<string>(ViewStereotypeName, ViewNameProperty)?.Trim();

            return string.IsNullOrWhiteSpace(configuredName)
                ? Model.Name.Pluralize()
                : configuredName;
        }

        private string GetFullViewName()
        {
            return $"[{GetSchemaName()}].[{GetViewName()}]";
        }

        private string GenerateCommentedExampleSelect()
        {
            var lines = new List<string>();

            if (Model.Attributes.Any())
            {
                for (var index = 0; index < Model.Attributes.Count; index++)
                {
                    var attribute = Model.Attributes[index];
                    var suffix = index < Model.Attributes.Count - 1 ? "," : string.Empty;
                    lines.Add($"    --     [{attribute.Name}]{suffix}");
                }
            }
            else
            {
                lines.Add("    --     [Field1]");
            }

            lines.Add($"    -- FROM [{GetSchemaName()}].[Table]");

            return string.Join(Environment.NewLine, lines);
        }
    }
}
