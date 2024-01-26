using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution
{
    [Description("Visual Studio 2015 Solution- VS Projects")]
    public class Registrations : IApplicationTemplateRegistration
    {
        public string TemplateId => VisualStudioSolutionTemplate.Identifier;
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(IApplicationTemplateInstanceRegistry registry, IApplication application)
        {
            var vsSolutions = _metadataManager.VisualStudio(application).GetVisualStudioSolutionModels();
            foreach (var vsSolution in vsSolutions)
            {
                var projects = _metadataManager
                    .GetAllProjectModels(application)
                    .Cast<IVisualStudioSolutionProject>()
                    .Concat(_metadataManager.VisualStudio(application).GetJavaScriptProjectModels())
                    .Where(x => x.Solution.Id == vsSolution.Id)
                    .ToList();

                registry.RegisterApplicationTemplate(VisualStudioSolutionTemplate.Identifier, () => new VisualStudioSolutionTemplate(application, vsSolution, projects));
            }
        }
    }
}
