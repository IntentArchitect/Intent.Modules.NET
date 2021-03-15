using System.Collections.Generic;
using System.ComponentModel;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    [Description(ServiceImplementationTemplate.Identifier)]
    public class ServiceImplementationTemplateRegistrations : FilePerModelTemplateRegistration<ServiceModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceImplementationTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceImplementationTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ServiceModel model)
        {
            return new ServiceImplementationTemplate(outputTarget, model);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetServiceModels();
        }
    }
}

