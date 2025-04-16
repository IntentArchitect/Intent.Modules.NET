using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
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
                fileExtension: "sql"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $"""
                    
                    """;
        }

    }
}