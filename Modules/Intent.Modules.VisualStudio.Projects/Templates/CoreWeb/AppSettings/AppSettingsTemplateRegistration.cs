using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
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
            var projects = _metadataManager.VisualStudio(applicationManager).GetASPNETCoreWebApplicationModels()
                .Select(x => new
                {
                    x.Id,
                    x.RuntimeEnvironments,
                    Location = "",
                    RequiresSpecifiedRole = false,
                    IncludeAllowHosts = true,
                    IncludeAspNetCoreLoggingLevel = true,

                })
                .Union(_metadataManager.VisualStudio(applicationManager).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkWeb())
                    .Select(x => new
                    {
                        x.Id,
                        x.RuntimeEnvironments,
                        Location = "",
                        RequiresSpecifiedRole = false,
                        IncludeAllowHosts = true,
                        IncludeAspNetCoreLoggingLevel = true,
                    }))
                .Union(_metadataManager.VisualStudio(applicationManager).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkWorker())
                    .Select(x => new
                    {
                        x.Id,
                        x.RuntimeEnvironments,
                        Location = "",
                        RequiresSpecifiedRole = false,
                        IncludeAllowHosts = false,
                        IncludeAspNetCoreLoggingLevel = false,
                    }))
                .Union(_metadataManager.VisualStudio(applicationManager).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkBlazorWebAssembly())
                    .Select(x => new
                    {
                        x.Id,
                        x.RuntimeEnvironments,
                        Location = "wwwroot",
                        RequiresSpecifiedRole = true,
                        IncludeAllowHosts = true,
                        IncludeAspNetCoreLoggingLevel = false,
                    }));

            foreach (var aspProject in projects)
            {
                var outputTarget = applicationManager.OutputTargets.Single(x => x.Id == aspProject.Id);

                registry.RegisterTemplate(TemplateId, outputTarget, target => new AppSettingsTemplate(target, new AppSettingsModel(null, aspProject.Location, aspProject.RequiresSpecifiedRole, aspProject.IncludeAllowHosts, aspProject.IncludeAspNetCoreLoggingLevel)));

                foreach (var runtimeEnvironment in aspProject.RuntimeEnvironments)
                {
                    registry.RegisterTemplate(TemplateId, outputTarget, target => new AppSettingsTemplate(target, new AppSettingsModel(runtimeEnvironment, aspProject.Location, aspProject.RequiresSpecifiedRole, aspProject.IncludeAllowHosts, aspProject.IncludeAspNetCoreLoggingLevel)));
                }
            }
        }
    }
}
