using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory.Templates.DesignTimeDbMigrationsReadMe
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DesignTimeDbMigrationsReadMeTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DesignTimeDbContextFactory.DesignTimeDbMigrationsReadMe";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DesignTimeDbMigrationsReadMeTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public string BoundedContextName => OutputTarget.ApplicationName();
        public string MigrationProject => OutputTarget.GetProject().Name;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "DESIGN_TIME_MIGRATION_README",
                fileExtension: "txt",
                relativeLocation: "");
        }
    }
}