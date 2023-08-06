using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modelers.WebClient.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Modelers.Types.ServiceProxies;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.EnumContract
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

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, EnumModel model)
        {
            return new EnumContractTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<EnumModel> GetModels(IApplication application)
        {
            return _metadataManager.WebClient(application).GetMappedServiceProxyEnumModels();
        }
    }
}