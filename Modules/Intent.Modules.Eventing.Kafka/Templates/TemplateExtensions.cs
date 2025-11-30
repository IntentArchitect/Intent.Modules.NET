using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConfiguration;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConsumer;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConsumerBackgroundService;
using Intent.Modules.Eventing.Kafka.Templates.KafkaConsumerInterface;
using Intent.Modules.Eventing.Kafka.Templates.KafkaEventDispatcher;
using Intent.Modules.Eventing.Kafka.Templates.KafkaEventDispatcherInterface;
using Intent.Modules.Eventing.Kafka.Templates.KafkaMessageBus;
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

        public static string GetKafkaEventDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaEventDispatcherTemplate.TemplateId);
        }

        public static string GetKafkaEventDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaEventDispatcherInterfaceTemplate.TemplateId);
        }

        public static string GetKafkaMessageBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(KafkaMessageBusTemplate.TemplateId);
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