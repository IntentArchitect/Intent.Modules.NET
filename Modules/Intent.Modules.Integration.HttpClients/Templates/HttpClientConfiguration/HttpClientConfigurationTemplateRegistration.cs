using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HttpClientConfigurationTemplateRegistration : SingleFileListModelTemplateRegistration<ServiceProxyModel>
    {
        private readonly IMetadataManager _metadataManager;

        public HttpClientConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (application.OutputTargets.First().GetProject().TargetFramework().StartsWith("netstandard"))
            {
                AbortRegistration(); // Need cleaner, more obvious way, to do this
                return;
            }
            base.DoRegistration(registry, application);
        }

        public override string TemplateId => HttpClientConfigurationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<ServiceProxyModel> model)
        {
            return new HttpClientConfigurationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ServiceProxyModel> GetModels(IApplication application)
        {
            return _metadataManager.ServiceProxies(application).GetServiceProxyModels()
                .Union(_metadataManager.Services(application).GetServiceProxyModels())
                .Where(p => p.GetMappedEndpoints().Any())
                .ToArray();
        }
    }
}