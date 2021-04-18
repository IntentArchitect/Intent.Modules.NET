using System.Collections.Generic;
using System.ComponentModel;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceImplementation
{
    [Description(ServiceImplementationTemplate.Identifier)]
    public class ServiceImplementationTemplateRegistrations : FilePerModelTemplateRegistration<ServiceModel>
    {
        private readonly IMetadataManager _metadataProvider;

        public ServiceImplementationTemplateRegistrations(IMetadataManager metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public override string TemplateId => ServiceImplementationTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ServiceModel model)
        {
            return new ServiceImplementationTemplate(outputTarget, model);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            return _metadataProvider.Services(application).GetServiceModels();
        }
    }
}

