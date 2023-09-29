using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe
{
    [IntentManaged(Mode.Merge)]
    partial class DbMigrationsReadMeTemplate : IntentTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DbMigrationsReadMe";
        public const string Identifier = "Intent.EntityFrameworkCore.DbMigrationsReadMe"; // Anything using this?

        [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
        public DbMigrationsReadMeTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public bool IncludeStartupProjectArguments { get; set; } = true;
        public bool IncludeDbContextArguments { get; set; } = false;
        public List<string> ExtraArguments { get; } = new();
        public string ExtraComments { get; set; } = string.Empty;
        
        public string BoundedContextName => OutputTarget.ApplicationName();
        public string MigrationProject => OutputTarget.GetProject().Name;
        public string StartupProject => ExecutionContext.OutputTargets.FirstOrDefault(x => x.Type == VisualStudioProjectTypeIds.CoreWebApp)?.Name ?? "UNKNOWN";
        public string DbContext => ExecutionContext.FindTemplateInstance<IClassProvider>(DbContextTemplate.TemplateId)?.ClassName ?? "UNKNOWN"; 

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(new DbMigrationsReadMeCreatedEvent(this));
        }

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
                NugetPackages.EntityFrameworkCoreDesign(OutputTarget),
                NugetPackages.EntityFrameworkCoreTools(OutputTarget)
            };
        }

        private string GetVsStartupProjectArgument()
        {
            return IncludeStartupProjectArguments
                ? $@"-StartupProject ""{StartupProject}"" "
                : string.Empty;
        }
        
        private string GetCliStartupProjectArgument()
        {
            return IncludeStartupProjectArguments
                ? $@"--startup-project ""{StartupProject}"" "
                : string.Empty;
        }

        private string GetVsDbContextArgument()
        {
            return IncludeDbContextArguments
                ? $@" -Context ""{DbContext}"" "
                : string.Empty;
        }
        
        private string GetCliDbContextArgument()
        {
            return IncludeDbContextArguments
                ? $@" --context ""{DbContext}"""
                : string.Empty;
        }

        private string GetExtraArguments()
        {
            if (!ExtraArguments.Any())
            {
                return string.Empty;
            }

            return " -- " + string.Join(" ", ExtraArguments);
        }

        private string GetExtraComments()
        {
            if (string.IsNullOrWhiteSpace(ExtraComments))
            {
                return string.Empty;
            }

            return Environment.NewLine + ExtraComments + Environment.NewLine;
        }
    }
}
