using System.Collections.Generic;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Eventing.Contracts.Templates.AssemblyAttributes;
using Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBus;
using Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBusConfiguration;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandler;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.Contracts.Templates.MessageBrokerRegistry;
using Intent.Modules.Eventing.Contracts.Templates.MessageBrokerResolver;
using Intent.Modules.Eventing.Contracts.Templates.MessageBusInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAssemblyAttributesName(this IIntentTemplate template)
        {
            return template.GetTypeName(AssemblyAttributesTemplate.TemplateId);
        }

        public static string GetCompositeMessageBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(CompositeMessageBusTemplate.TemplateId);
        }

        public static string GetCompositeMessageBusConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(CompositeMessageBusConfigurationTemplate.TemplateId);
        }

        public static string GetEventBusInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(EventBusInterfaceTemplate.TemplateId);
        }

        public static string GetIntegrationCommandName<T>(this IIntentTemplate<T> template) where T : IntegrationCommandModel
        {
            return template.GetTypeName(IntegrationCommandTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationCommandName(this IIntentTemplate template, IntegrationCommandModel model)
        {
            return template.GetTypeName(IntegrationCommandTemplate.TemplateId, model);
        }

        public static string GetIntegrationEventDtoName<T>(this IIntentTemplate<T> template) where T : EventingDTOModel
        {
            return template.GetTypeName(IntegrationEventDtoTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventDtoName(this IIntentTemplate template, EventingDTOModel model)
        {
            return template.GetTypeName(IntegrationEventDtoTemplate.TemplateId, model);
        }

        public static string GetIntegrationEventEnumName<T>(this IIntentTemplate<T> template) where T : EnumModel
        {
            return template.GetTypeName(IntegrationEventEnumTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventEnumName(this IIntentTemplate template, EnumModel model)
        {
            return template.GetTypeName(IntegrationEventEnumTemplate.TemplateId, model);
        }

        public static string GetIntegrationEventHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, model);
        }

        public static string GetIntegrationEventHandlerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(IntegrationEventHandlerInterfaceTemplate.TemplateId);
        }

        public static string GetIntegrationEventMessageName<T>(this IIntentTemplate<T> template) where T : MessageModel
        {
            return template.GetTypeName(IntegrationEventMessageTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventMessageName(this IIntentTemplate template, MessageModel model)
        {
            return template.GetTypeName(IntegrationEventMessageTemplate.TemplateId, model);
        }

        public static string GetMessageBrokerRegistryName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageBrokerRegistryTemplate.TemplateId);
        }

        public static string GetMessageBrokerResolverName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageBrokerResolverTemplate.TemplateId);
        }

        public static string GetMessageBusInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(MessageBusInterfaceTemplate.TemplateId);
        }
    }
}