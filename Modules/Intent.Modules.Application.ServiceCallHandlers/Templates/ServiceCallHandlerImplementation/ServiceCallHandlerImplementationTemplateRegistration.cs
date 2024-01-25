using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandlerImplementation
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ServiceCallHandlerImplementationTemplateRegistration : FilePerModelTemplateRegistration<OperationModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceCallHandlerImplementationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceCallHandlerImplementationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, OperationModel model)
        {
            return new ServiceCallHandlerImplementationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<OperationModel> GetModels(IApplication application)
        {
            var serviceModels = _metadataManager.Services(application).GetServiceModels()
                .SelectMany(s => s.Operations)
                .ToList();
            return serviceModels;
        }
    }
}