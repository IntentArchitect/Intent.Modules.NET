using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.HttpClientFake
{
    [IntentManaged(Mode.Ignore)]
    public class HttpClientFakeTemplateRegistration : FilePerModelTemplateRegistration<IServiceProxyModel>
    {
        private readonly IMetadataManager _metadataManager;

        public HttpClientFakeTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => HttpClientFakeTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IServiceProxyModel model)
        {
            return new HttpClientFakeTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IEnumerable<IServiceProxyModel> GetModels(IApplication application)
        {
            const string serviceProxiesDesignerId = "2799aa83-e256-46fe-9589-b96f7d6b09f7";
            return _metadataManager.GetServiceProxyModels(
                application.Id,
                application,
                applicationId => _metadataManager.GetDesigner(applicationId, serviceProxiesDesignerId),
                _metadataManager.Services);
        }
    }
}
