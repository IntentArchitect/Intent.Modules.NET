using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    [Description("Visual Studio 2015 Solution- VS Projects")]
    public class Registrations : IApplicationTemplateRegistration
    {
        public string TemplateId => VisualStudio2015SolutionTemplate.Identifier;
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

                registry.RegisterApplicationTemplate(VisualStudio2015SolutionTemplate.Identifier, () => new VisualStudio2015SolutionTemplate(application, vsSolution, projects));
            }
        }
    }
}
