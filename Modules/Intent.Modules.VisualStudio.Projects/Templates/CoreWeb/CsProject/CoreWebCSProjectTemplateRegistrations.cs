using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject
{
    [Description(CoreWebCSProjectTemplate.TemplateId)]
    public class CoreWebCSProjectTemplateRegistrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => CoreWebCSProjectTemplate.TemplateId;

        public CoreWebCSProjectTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.CodebaseStructure(application).GetASPNETCoreWebApplicationModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.RegisterTemplate(TemplateId, project, p => new CoreWebCSProjectTemplate(project, model));
            }
        }
    }
}
