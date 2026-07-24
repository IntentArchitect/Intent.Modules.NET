using System.Linq;
using Intent.Engine;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceWorker.ServiceWorkerProgram
{
    public class ServiceWorkerProgramTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceWorkerProgramTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => ServiceWorkerProgramTemplate.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.CodebaseStructure(application).GetCSharpProjectNETModels().Where(x => x.GetNETSettings()?.SDK()?.IsMicrosoftNETSdkWorker() == true);

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new ServiceWorkerProgramTemplate(p, model));
            }
        }
    }
}