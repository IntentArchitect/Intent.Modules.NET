using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.ServiceContract
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
            return _metadataManager.UserInterface(application).GetServiceProxyModels()
                .Where(x => x.HasMappedEndpoints())
                .ToArray();
        }
    }
}