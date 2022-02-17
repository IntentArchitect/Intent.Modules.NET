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

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

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

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var aspProjects = _metadataManager.VisualStudio(application).GetASPNETCoreWebApplicationModels();

            foreach (var aspProject in aspProjects)
            {
                var outputTarget = application.OutputTargets.Single(x => x.Id == aspProject.Id);

                registry.RegisterTemplate(TemplateId, outputTarget, target => new AppSettingsTemplate(target, new AppSettingsModel(aspProject)));

                foreach (var runtimeEnvironment in aspProject.RuntimeEnvironments)
                {
                    registry.RegisterTemplate(TemplateId, outputTarget, target => new AppSettingsTemplate(target, new AppSettingsModel(aspProject, runtimeEnvironment)));
                }
            }
        }
    }
}
