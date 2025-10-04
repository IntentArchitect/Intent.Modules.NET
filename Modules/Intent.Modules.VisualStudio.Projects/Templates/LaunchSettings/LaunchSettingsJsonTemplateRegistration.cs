using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Persistence;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.LaunchSettings
{
    [Description(LaunchSettingsJsonTemplate.Identifier)]
    public class LaunchSettingsJsonTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private readonly IPersistenceLoader _persistenceLoader;
        public string TemplateId => LaunchSettingsJsonTemplate.Identifier;

        public LaunchSettingsJsonTemplateRegistration(IMetadataManager metadataManager, IPersistenceLoader persistenceLoader)
        {
            _metadataManager = metadataManager;
            _persistenceLoader = persistenceLoader;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var modelIds = _metadataManager.VisualStudio(application).GetASPNETCoreWebApplicationModels()
                .Select(x => x.Id)
                .Union(_metadataManager.VisualStudio(application).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkWeb() || 
                                x.GetNETSettings().SDK().IsMicrosoftNETSdkBlazorWebAssembly() || 
                                x.GetNETSettings().SDK().IsMicrosoftNETSdkWorker() ||
                                (x.GetNETSettings().SDK().IsMicrosoftNETSdk() && x.GetNETSettings().GenerateLaunchSettingsFile()))
                    .Select(x => x.Id));

            foreach (var modelId in modelIds)
            {
                var project = application.Projects.Single(x => x.Id == modelId);
                registry.Register(TemplateId, project, p => new LaunchSettingsJsonTemplate(p, project.Application.EventDispatcher, application, _persistenceLoader));
            }
        }
    }
}
