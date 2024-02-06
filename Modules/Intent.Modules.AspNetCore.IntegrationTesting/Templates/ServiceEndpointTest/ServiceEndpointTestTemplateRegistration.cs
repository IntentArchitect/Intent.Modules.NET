using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.ServiceEndpointTest
{
    [IntentManaged(Mode.Ignore)]
    public class ServiceEndpointTestTemplateRegistration : FilePerModelTemplateRegistration<IHttpEndpointModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceEndpointTestTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceEndpointTestTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IHttpEndpointModel model)
        {
            return new ServiceEndpointTestTemplate(outputTarget, model);
        }

        public override IEnumerable<IHttpEndpointModel> GetModels(IApplication application)
        {
            return _metadataManager.GetServicesAsProxyModels(application).SelectMany(s => s.GetMappedEndpoints());
        }
    }
}