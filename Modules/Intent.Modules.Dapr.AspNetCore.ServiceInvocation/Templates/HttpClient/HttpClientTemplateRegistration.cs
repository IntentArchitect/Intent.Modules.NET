using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient
{
    [IntentManaged(Mode.Ignore)]
    public class HttpClientTemplateRegistration : FilePerModelTemplateRegistration<IServiceProxyModel>
    {
        private readonly IMetadataManager _metadataManager;

        public HttpClientTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => HttpClientTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IServiceProxyModel model)
        {
            return new HttpClientTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IServiceProxyModel> GetModels(IApplication application)
        {
            var proxyModels = _metadataManager.ServiceProxies(application).GetServiceProxyModels()
                .Where(p => p.HasMappedEndpoints());
            return proxyModels.Select(p => new ServiceProxyModelAdapter(p, ProxySettingsHelper.GetSerializeEnumsAsStrings(application, p) == true));
        }
    }
}