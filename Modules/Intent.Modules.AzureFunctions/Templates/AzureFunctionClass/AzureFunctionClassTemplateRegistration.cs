using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureFunctionClassTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionClassTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => AzureFunctionClassTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var models = _metadataManager.Services(applicationManager)
                .GetServiceModels()
                .SelectMany(s => s.Operations)
                .ToArray();

            var serviceSet = new HashSet<ServiceModel>();
            models.ToList().ForEach(x => serviceSet.Add(x.ParentService));
            var hasMultipleServices = serviceSet.Count > 1;

            foreach (var model in models)
            {
                registry.RegisterTemplate(
                    templateId: TemplateId,
                    createTemplateInstance: project => new AzureFunctionClassTemplate(project, model, hasMultipleServices));
            }
        }
    }
}