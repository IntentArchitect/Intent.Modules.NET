using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    [Description("Intent Applications Service Contracts (Clients)")]
    public class ServiceContractRegistrations : FilePerModelTemplateRegistration<ServiceProxyModel>
    {
        private readonly IMetadataManager _metadataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "CSharp";

        public ServiceContractRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TemplateIds.ClientServiceContract;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ServiceProxyModel model)
        {
            return new ServiceContractTemplate(outputTarget, new ServiceModel((IElement) model.Mapping.Element), TemplateId);
        }

        public override IEnumerable<ServiceProxyModel> GetModels(IApplication application)
        {
            return _metadataManager.ServiceProxies(application).GetServiceProxyModels();
        }
    }
}