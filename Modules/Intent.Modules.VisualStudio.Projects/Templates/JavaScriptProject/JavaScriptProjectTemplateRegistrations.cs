using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;


namespace Intent.Modules.VisualStudio.Projects.Templates.JavaScriptProject
{
    public class JavaScriptProjectTemplateRegistrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public string TemplateId => JavaScriptProjectTemplate.TemplateId;

        public JavaScriptProjectTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetJavaScriptProjectModels();

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new JavaScriptProjectTemplate(p, model));
            }
        }
    }
}
