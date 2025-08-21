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
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DbMigrationsReadMeTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DbMigrationsReadMe";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DbMigrationsReadMeTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            IncludeDbContextArguments = DbContextManager.GetDbContexts(ExecutionContext.GetApplicationConfig().Id, ExecutionContext.MetadataManager).Count > 1;
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            //If we have DesignTimeDbContextFactory installed you need to use the CLI
            PackageManagerConsoleSupported = !this.TryGetTypeName("Intent.EntityFrameworkCore.DesignTimeDbContextFactory.DesignTimeDbContextFactory", out var _);
        }

        public bool PackageManagerConsoleSupported { get; set; } = true;
        public bool IncludeStartupProjectArguments { get; set; } = true;
        public bool IncludeDbContextArguments { get; set; }
        public List<string> ExtraArguments { get; } = new();
        public string ExtraComments { get; set; } = string.Empty;

        public string BoundedContextName => OutputTarget.ApplicationName();
        public string MigrationProject => OutputTarget.GetProject().Name;
        //Backwards compatible
        public string StartupProject => ExecutionContext.OutputTargets.Where(x => x.Type != "Folder").FirstOrDefault(x =>
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
            return base.CanRunTemplate() && DbContextManager.GetDbContexts(this.ExecutionContext.GetApplicationConfig().Id, ExecutionContext.MetadataManager).Any();
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"README",
                fileExtension: "md"
            );
        }

    }
}