using System.Collections.Generic;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts.DtoContract;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts.EnumContract;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts.PagedResult;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts.ServiceContract;
using Intent.Modules.Eventing.MassTransit.Templates.ClientImplementation.ServiceRequestClient;
using Intent.Modules.Eventing.MassTransit.Templates.FinbuckleConsumingFilter;
using Intent.Modules.Eventing.MassTransit.Templates.FinbuckleMessageHeaderStrategy;
using Intent.Modules.Eventing.MassTransit.Templates.FinbucklePublishingFilter;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventConsumer;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandler;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandlerImplementation;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitEventBus;
using Intent.Modules.Eventing.MassTransit.Templates.MediatRConsumer;
using Intent.Modules.Eventing.MassTransit.Templates.RequestResponse.MapperRequestInterface;
using Intent.Modules.Eventing.MassTransit.Templates.RequestResponse.MapperRequestMessage;
using Intent.Modules.Eventing.MassTransit.Templates.RequestResponse.RequestCompletedMessage;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDtoContractName<T>(this IIntentTemplate<T> template) where T : DTOModel
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, template.Model);
        }

        public static string GetDtoContractName(this IIntentTemplate template, DTOModel model)
        {
            return template.GetTypeName(DtoContractTemplate.TemplateId, model);
        }

        public static string GetEnumContractName<T>(this IIntentTemplate<T> template) where T : EnumModel
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, template.Model);
        }

        public static string GetEnumContractName(this IIntentTemplate template, EnumModel model)
        {
            return template.GetTypeName(EnumContractTemplate.TemplateId, model);
        }

        public static string GetPagedResultName(this IIntentTemplate template)
        {
            return template.GetTypeName(PagedResultTemplate.TemplateId);
        }

        public static string GetServiceContractName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, template.Model);
        }

        public static string GetServiceContractName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(ServiceContractTemplate.TemplateId, model);
        }

        public static string GetServiceRequestClientName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(ServiceRequestClientTemplate.TemplateId, template.Model);
        }

        public static string GetServiceRequestClientName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(ServiceRequestClientTemplate.TemplateId, model);
        }

        public static string GetFinbuckleConsumingFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(FinbuckleConsumingFilterTemplate.TemplateId);
        }

        public static string GetFinbuckleMessageHeaderStrategyName(this IIntentTemplate template)
        {
            return template.GetTypeName(FinbuckleMessageHeaderStrategyTemplate.TemplateId);
        }
        public static string GetFinbucklePublishingFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(FinbucklePublishingFilterTemplate.TemplateId);
        }

        public static string GetIntegrationEventConsumerName(this IIntentTemplate template)
        {
            return template.GetTypeName(IntegrationEventConsumerTemplate.TemplateId);
        }

        public static string GetIntegrationEventHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, model);
        }
        public static string GetIntegrationEventHandlerImplementationName<T>(this IIntentTemplate<T> template) where T : MessageSubscribeAssocationTargetEndModel
        {
            return template.GetTypeName(IntegrationEventHandlerImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerImplementationName(this IIntentTemplate template, MessageSubscribeAssocationTargetEndModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerImplementationTemplate.TemplateId, model);
        }

        public static string GetMassTransitConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(MassTransitConfigurationTemplate.TemplateId);
        }

        public static string GetMassTransitEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(MassTransitEventBusTemplate.TemplateId);
        }

        public static string GetMediatRConsumerName(this IIntentTemplate template)
        {
            return template.GetTypeName(MediatRConsumerTemplate.TemplateId);
        }

        public static string GetMapperRequestInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(MapperRequestInterfaceTemplate.TemplateId);
        }

        public static string GetMapperRequestMessageName<T>(this IIntentTemplate<T> template)
where T : CommandQueryModel
        {
            return template.GetTypeName(MapperRequestMessageTemplate.TemplateId, template.Model);
        }

        public static string GetMapperRequestMessageName(this IIntentTemplate template, CommandQueryModel model)
        {
            return template.GetTypeName(MapperRequestMessageTemplate.TemplateId, model);
        }

        public static string GetRequestCompletedMessageName(this IIntentTemplate template)
        {
            return template.GetTypeName(RequestCompletedMessageTemplate.TemplateId);
        }
    }
}