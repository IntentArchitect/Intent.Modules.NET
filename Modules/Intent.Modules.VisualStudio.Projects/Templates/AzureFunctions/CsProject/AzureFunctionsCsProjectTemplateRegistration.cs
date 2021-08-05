using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctions.CsProject
{
    public class AzureFunctionsCsProjectTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionsCsProjectTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetAzureFunctionsProjectModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new AzureFunctionsCsProjectTemplate(p, model));
            }

        }

        public string TemplateId => AzureFunctionsCsProjectTemplate.TemplateId;
    }
}