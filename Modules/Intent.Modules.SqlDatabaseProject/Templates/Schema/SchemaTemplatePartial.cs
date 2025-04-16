using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.SqlDatabaseProject.Templates.Schema
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class SchemaTemplate : IntentTemplateBase<SchemaModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.SqlDatabaseProject.SchemaTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SchemaTemplate(IOutputTarget outputTarget, SchemaModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "sql"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $"CREATE SCHEMA [{Model.Name}]";
        }
    }
}