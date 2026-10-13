using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AppSettingsTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => AppSettingsTemplate.TemplateId;

        public AppSettingsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            // Web, Worker and Blazor WebAssembly projects are always registered: managed when "Generate
            // AppSettings File" is on, hands-off when it is off, so that settings raised for the project are
            // still claimed by it rather than falling through to another project's file. A plain
            // Microsoft.NET.Sdk project is only registered when the flag is on: when off, its settings are
            // meant to flow on to the host projects that reference it.
            var projects = _metadataManager.CodebaseStructure(applicationManager).GetASPNETCoreWebApplicationModels()
                .Select(x => new
                {
                    x.Id,
                    x.RuntimeEnvironments,
                    Location = "",
                    RequiresSpecifiedRole = false,
                    IncludeAllowHosts = true,
                    IncludeAspNetCoreLoggingLevel = true,
                    IsHandsOff = false,
                })
                .Union(_metadataManager.CodebaseStructure(applicationManager).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkWeb())
                    .Select(x => new
                    {
                        x.Id,
                        x.RuntimeEnvironments,
                        Location = "",
                        RequiresSpecifiedRole = false,
                        IncludeAllowHosts = true,
                        IncludeAspNetCoreLoggingLevel = true,
                        IsHandsOff = !x.GetNETSettings().GenerateAppSettingsFile(),
                    }))
                .Union(_metadataManager.CodebaseStructure(applicationManager).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkWorker())
                    .Select(x => new
                    {
                        x.Id,
                        x.RuntimeEnvironments,
                        Location = "",
                        RequiresSpecifiedRole = false,
                        IncludeAllowHosts = false,
                        IncludeAspNetCoreLoggingLevel = false,
                        IsHandsOff = !x.GetNETSettings().GenerateAppSettingsFile(),
                    }))
                .Union(_metadataManager.CodebaseStructure(applicationManager).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkBlazorWebAssembly())
                    .Select(x => new
                    {
                        x.Id,
                        x.RuntimeEnvironments,
                        Location = "wwwroot",
                        RequiresSpecifiedRole = true,
                        IncludeAllowHosts = true,
                        IncludeAspNetCoreLoggingLevel = false,
                        IsHandsOff = !x.GetNETSettings().GenerateAppSettingsFile(),
                    }))
                .Union(_metadataManager.CodebaseStructure(applicationManager).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdk() &&
                                x.GetNETSettings().GenerateAppSettingsFile())
                    .Select(x => new
                    {
                        x.Id,
                        x.RuntimeEnvironments,
                        Location = "",
                        RequiresSpecifiedRole = false,
                        IncludeAllowHosts = false,
                        IncludeAspNetCoreLoggingLevel = false,
                        IsHandsOff = false,
                    }));

            foreach (var aspProject in projects)
            {
                var outputTarget = applicationManager.OutputTargets.Single(x => x.Id == aspProject.Id);

                registry.RegisterTemplate(TemplateId, outputTarget, target => new AppSettingsTemplate(target, new AppSettingsModel(null, aspProject.Location, aspProject.RequiresSpecifiedRole, aspProject.IncludeAllowHosts, aspProject.IncludeAspNetCoreLoggingLevel, aspProject.IsHandsOff)));

                foreach (var runtimeEnvironment in aspProject.RuntimeEnvironments)
                {
                    registry.RegisterTemplate(TemplateId, outputTarget, target => new AppSettingsTemplate(target, new AppSettingsModel(runtimeEnvironment, aspProject.Location, aspProject.RequiresSpecifiedRole, aspProject.IncludeAllowHosts, aspProject.IncludeAspNetCoreLoggingLevel, aspProject.IsHandsOff)));
                }
            }
        }
    }
}
