using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.ServiceContract
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ServiceContractTemplateRegistration : FilePerModelTemplateRegistration<ServiceProxyModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceContractTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceContractTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ServiceProxyModel model)
        {
            return new ServiceContractTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ServiceProxyModel> GetModels(IApplication application)
        {
            var service = new MassTransitServiceProxyMappedService();
            return _metadataManager.ServiceProxies(application).GetServiceProxyModels()
                .Where(proxyModel => service.HasMappedEndpoints(proxyModel))
                .ToArray();
        }
    }
}