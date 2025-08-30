using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Modelers.Types.ServiceProxies;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.EnumContract
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class EnumContractTemplateRegistration : FilePerModelTemplateRegistration<EnumModel>
    {
        private readonly IMetadataManager _metadataManager;

        public EnumContractTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EnumContractTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, EnumModel model)
        {
            return new EnumContractTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<EnumModel> GetModels(IApplication application)
        {
            const string serviceProxiesDesignerId = "2799aa83-e256-46fe-9589-b96f7d6b09f7";
            var results = _metadataManager
                .GetServiceProxyReferencedEnums(
                    applicationId: application.Id,
                    stereotypeNames: null,
                    getDesigners:
                    [
                        applicationId => _metadataManager.GetDesigner(applicationId, serviceProxiesDesignerId), // for backward compatibility
                        _metadataManager.Services
                    ])
                .ToArray();

            return results;
        }
    }
}