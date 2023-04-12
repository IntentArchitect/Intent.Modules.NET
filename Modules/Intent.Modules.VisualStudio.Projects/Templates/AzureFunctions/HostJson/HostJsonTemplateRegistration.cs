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

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctions.HostJson
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HostJsonTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public HostJsonTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => HostJsonTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var azureFunctionsProjectModelIds = _metadataManager.VisualStudio(applicationManager)
                .GetAzureFunctionsProjectModels()
                .Select(x => x.Id);

            var genericProjectModelIds = _metadataManager.VisualStudio(applicationManager)
                .GetCSharpProjectNETModels()
                .Where(x => x.TryGetNETSettings(out var s) && !string.IsNullOrWhiteSpace(s.AzureFunctionsVersion().Value))
                .Select(x => x.Id);

            foreach (var projectModelId in azureFunctionsProjectModelIds.Union(genericProjectModelIds))
            {
                var project = applicationManager.OutputTargets.First(x => x.Id == projectModelId);
                registry.RegisterTemplate(TemplateId, project, p => new HostJsonTemplate(p));
            }
        }
    }
}