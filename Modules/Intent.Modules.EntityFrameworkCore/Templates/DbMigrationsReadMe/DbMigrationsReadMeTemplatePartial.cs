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
            IncludeDbContextArguments = DbContextManager.GetDbContexts(ExecutionContext.GetApplicationConfig().Id, ExecutionContext.MetadataManager).Count > 1;
        }

        public bool IncludeStartupProjectArguments { get; set; } = true;
        public bool IncludeDbContextArguments { get; set; }
        public List<string> ExtraArguments { get; } = new();
        public string ExtraComments { get; set; } = string.Empty;

        public string BoundedContextName => OutputTarget.ApplicationName();
        public string MigrationProject => OutputTarget.GetProject().Name;
        //Backwards compatible
        public string StartupProject => ExecutionContext.OutputTargets.FirstOrDefault(x =>
        {
            if (x.Type == VisualStudioProjectTypeIds.CoreWebApp)
            {
                return true;
            }
            if (x.GetProject() is IHasStereotypes stereotypes && stereotypes.HasStereotype(".NET Settings"))
            {
                var settings = stereotypes.GetStereotype(".NET Settings");
                switch (settings.GetProperty("SDK").Value)
                {
                    case "Microsoft.NET.Sdk.Web":
                    case "Microsoft.NET.Sdk.Worker":
                        return true;
                    default:
                        break;
                }

                var outputType = settings.GetProperty("Output Type");
                if (outputType != null)
                {
                    switch (outputType.Value)
                    {
                        case "Console Application":
                        case "Windows Application":
                            return true;
                        default:
                            break;
                    }
                }
                var azureFunctionVersion = settings.GetProperty("Azure Functions Version");
                if (azureFunctionVersion != null && !string.IsNullOrEmpty(azureFunctionVersion.Value))
                {
                    return true;
                }
            }
            return false;
        })?.Name ?? "UNKNOWN";
        public string DbContext => ExecutionContext.FindTemplateInstance<IClassProvider>(TemplateRoles.Infrastructure.Data.DbContext)?.ClassName ?? "DBCONTEXT_NAME";

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(new DbMigrationsReadMeCreatedEvent(this));
        }

        public override bool CanRunTemplate()
        {
            return DbContextManager.GetDbContexts(this.ExecutionContext.GetApplicationConfig().Id, ExecutionContext.MetadataManager).Any();
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
                NugetPackages.MicrosoftEntityFrameworkCoreDesign(OutputTarget),
                NugetPackages.MicrosoftEntityFrameworkCoreTools(OutputTarget)
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
