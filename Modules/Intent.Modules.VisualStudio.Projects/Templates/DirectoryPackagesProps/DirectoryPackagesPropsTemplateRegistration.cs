using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.DirectoryPackagesProps
{
    public class DirectoryPackagesPropsTemplateRegistration : IApplicationTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public DirectoryPackagesPropsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => DirectoryPackagesPropsTemplate.TemplateId;

        public void DoRegistration(IApplicationTemplateInstanceRegistry registry, IApplication application)
        {
            var vsSolutions = _metadataManager.VisualStudio(application).GetVisualStudioSolutionModels();
            foreach (var vsSolution in vsSolutions)
            {
                var projects = _metadataManager.GetAllProjectModels(application).Where(x => x.Solution.Id == vsSolution.Id).ToList();
                registry.RegisterApplicationTemplate(TemplateId, () => new DirectoryPackagesPropsTemplate(application, vsSolution));
            }
        }
    }
}