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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctions.LocalSettingsJson
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class LocalSettingsJsonTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public LocalSettingsJsonTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => LocalSettingsJsonTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var models = _metadataManager.VisualStudio(applicationManager)
                .GetAzureFunctionsProjectModels();

            foreach (var model in models)
            {
                var project = applicationManager.OutputTargets.First(x => x.Id == model.Id);
                registry.RegisterTemplate(TemplateId, project, p => new LocalSettingsJsonTemplate(p));
            }
        }
    }
}