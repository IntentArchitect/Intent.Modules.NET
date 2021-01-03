using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Templates;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System;

[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe
{
    [IntentManaged(Mode.Merge)]
    partial class DbMigrationsReadMeTemplate : IntentTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.EntityFrameworkCore.DbMigrationsReadMe";


        public DbMigrationsReadMeTemplate(IOutputTarget project)
            : base(Identifier, project, null)
        {
        }

        public string BoundedContextName => OutputTarget.ApplicationName();
        public string MigrationProject => OutputTarget.Name;
        public string StartupProject => OutputTarget.Application.OutputTargets.FirstOrDefault(x => x.Type == VisualStudioProjectTypeIds.CoreWebApp)?.Name ?? "UNKNOWN";

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "MIGRATION_README",
                fileExtension: "txt",
                relativeLocation: "");
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFrameworkCoreDesign,
                NugetPackages.EntityFrameworkCoreTools,
            }
            .ToArray();
        }
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DbMigrationsReadMe";
    }
}
