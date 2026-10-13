using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.CodebaseStructure.Api;
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
            // Web, Worker and Blazor WebAssembly projects are always registered: managed when "Generate
            // LaunchSettings File" is on, hands-off (an existing file is kept exactly as it is) when off.
            // A plain Microsoft.NET.Sdk project is only registered when the flag is on.
            var registrations = _metadataManager.CodebaseStructure(application).GetASPNETCoreWebApplicationModels()
                .Select(x => (x.Id, IsHandsOff: false))
                .Union(_metadataManager.CodebaseStructure(application).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdkWeb() ||
                                x.GetNETSettings().SDK().IsMicrosoftNETSdkBlazorWebAssembly() ||
                                x.GetNETSettings().SDK().IsMicrosoftNETSdkWorker())
                    .Select(x => (x.Id, IsHandsOff: !x.GetNETSettings().GenerateLaunchSettingsFile())))
                .Union(_metadataManager.CodebaseStructure(application).GetCSharpProjectNETModels()
                    .Where(x => x.GetNETSettings().SDK().IsMicrosoftNETSdk() && x.GetNETSettings().GenerateLaunchSettingsFile())
                    .Select(x => (x.Id, IsHandsOff: false)));

            foreach (var (modelId, isHandsOff) in registrations)
            {
                var project = application.Projects.Single(x => x.Id == modelId);
                registry.Register(TemplateId, project, p => new LaunchSettingsJsonTemplate(p, project.Application.EventDispatcher, application, _persistenceLoader, isHandsOff));
            }
        }
    }
}
