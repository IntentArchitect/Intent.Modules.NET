using System.ComponentModel;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Registrations;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    [Description(ServiceCallHandlerImplementationTemplate.Identifier)]
    public class ServiceCallHandlerImplementationTemplateRegistrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceCallHandlerImplementationTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => ServiceCallHandlerImplementationTemplate.Identifier;

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var serviceModels = _metadataManager.Services(application).GetServiceModels();

            foreach (var serviceModel in serviceModels)
            {
                foreach (var operationModel in serviceModel.Operations)
                {
                    registry.RegisterTemplate(TemplateId, target => new ServiceCallHandlerImplementationTemplate(target, operationModel));
                }
            }
        }
    }
}
