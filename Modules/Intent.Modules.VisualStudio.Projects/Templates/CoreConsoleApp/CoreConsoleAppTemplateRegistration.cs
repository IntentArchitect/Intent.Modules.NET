using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreConsoleApp
{
    public class CoreConsoleAppTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public CoreConsoleAppTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetConsoleAppNETCoreModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new CoreConsoleAppTemplate(p, model));
            }

        }

        public string TemplateId => CoreConsoleAppTemplate.TemplateId;
    }
}