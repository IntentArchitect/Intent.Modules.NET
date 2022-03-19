using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.Build.Evaluation;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class SqlProjectTemplate : IntentTemplateBase<SQLServerDatabaseProjectModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.SqlServerDatabase.SqlProject";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SqlProjectTemplate(IOutputTarget outputTarget, SQLServerDatabaseProjectModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "sqlproj",
                overwriteBehaviour: OverwriteBehaviour.OnceOff
            );
        }

        public override void OnCreated()
        {
            base.OnCreated();
            ExecutionContext.EventDispatcher.Publish(new VisualStudioSolutionProjectCreatedEvent(OutputTarget.Id, GetMetadata().GetFilePath()));
        }
    }
}