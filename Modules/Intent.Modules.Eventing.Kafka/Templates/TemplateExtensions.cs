using System.Collections.Generic;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Kafka.Templates.IntegrationEventHandler;
using Intent.Modules.Eventing.Kafka.Templates.IntegrationEventHandlerImplementation;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConfiguration;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConsumer;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConsumerBackgroundService;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConsumerInterface;
using Intent.Modules.Eventing.Kafka.Templates.KafkaEventBus;
using Intent.Modules.Eventing.Kafka.Templates.KafkaEventDispatcher;
using Intent.Modules.Eventing.Kafka.Templates.KafkaEventDispatcherInterface;
using Intent.Modules.Eventing.Kafka.Templates.KafkaProducer;
using Intent.Modules.Eventing.Kafka.Templates.KafkaProducerInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates
{
    public static class TemplateExtensions
    {
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

        public static string GetKafkaConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaConfigurationTemplate.TemplateId);
        }

        public static string GetKafkaConsumerName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaConsumerTemplate.TemplateId);
        }

        public static string GetKafkaConsumerBackgroundServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaConsumerBackgroundServiceTemplate.TemplateId);
        }

        public static string GetKafkaConsumerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaConsumerInterfaceTemplate.TemplateId);
        }

        public static string GetKafkaEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaEventBusTemplate.TemplateId);
        }

        public static string GetKafkaEventDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaEventDispatcherTemplate.TemplateId);
        }

        public static string GetKafkaEventDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaEventDispatcherInterfaceTemplate.TemplateId);
        }

        public static string GetKafkaProducerName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaProducerTemplate.TemplateId);
        }

        public static string GetKafkaProducerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaProducerInterfaceTemplate.TemplateId);
        }

    }
}