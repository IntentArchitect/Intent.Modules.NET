using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.GitIgnore
{
    public class GitIgnoreTemplateRegistration : IApplicationTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public GitIgnoreTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => GitIgnoreTemplate.TemplateId;

        public void DoRegistration(IApplicationTemplateInstanceRegistry registry, IApplication application)
        {
            var vsSolutions = _metadataManager.VisualStudio(application).GetVisualStudioSolutionModels();
            foreach (var vsSolution in vsSolutions)
            {
                var projects = _metadataManager.GetAllProjectModels(application).Where(x => x.Solution.Id == vsSolution.Id).ToList();
                registry.RegisterApplicationTemplate(TemplateId, () => new GitIgnoreTemplate(application, vsSolution));
            }
        }
    }
}