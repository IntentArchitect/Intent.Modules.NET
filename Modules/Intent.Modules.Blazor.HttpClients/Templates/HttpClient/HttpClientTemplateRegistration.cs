using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.HttpClient
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
            return _metadataManager.GetServiceProxyModels(
                application.Id,
                _metadataManager.UserInterface);
        }
    }
}