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

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.PublishProfileCloud
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class PublishProfileCloudTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public PublishProfileCloudTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => PublishProfileCloudTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var models = _metadataManager.VisualStudio(applicationManager).GetServiceFabricProjectModels();

            foreach (var model in models)
            {
                var project = applicationManager.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new PublishProfileCloudTemplate(p, model));
            }
        }
    }
}