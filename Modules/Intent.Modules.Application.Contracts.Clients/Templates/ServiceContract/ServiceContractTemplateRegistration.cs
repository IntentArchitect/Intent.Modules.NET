using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ServiceContractTemplateRegistration : FilePerModelTemplateRegistration<IServiceContractModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceContractTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceContractTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IServiceContractModel model)
        {
            return new ServiceContractTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IServiceContractModel> GetModels(IApplication application)
        {
            const string serviceProxiesDesignerId = "2799aa83-e256-46fe-9589-b96f7d6b09f7";
            return _metadataManager.GetServiceContractModels(
                application.Id,
                applicationId => _metadataManager.GetDesigner(applicationId, serviceProxiesDesignerId), // for backward compatibility
                _metadataManager.Services);
        }
    }
}