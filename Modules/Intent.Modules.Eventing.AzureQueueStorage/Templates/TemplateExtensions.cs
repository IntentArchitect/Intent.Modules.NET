using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageConfiguration;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageConsumer;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageConsumerBackgroundService;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageConsumerInterface;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEnvelope;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEventBus;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEventDispatcher;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEventDispatcherInterface;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageOptions;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageSubscriptionOptions;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.IntegrationEventHandler;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureQueueStorageConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageConfigurationTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageConsumerName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageConsumerTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageConsumerBackgroundServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageConsumerBackgroundServiceTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageConsumerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageConsumerInterfaceTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageEnvelopeName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageEnvelopeTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageEventBusTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageEventDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageEventDispatcherTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageEventDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageEventDispatcherInterfaceTemplate.TemplateId);
        }
        public static string GetAzureQueueStorageOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageOptionsTemplate.TemplateId);
        }

        public static string GetAzureQueueStorageSubscriptionOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureQueueStorageSubscriptionOptionsTemplate.TemplateId);
        }

        public static string GetIntegrationEventHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, model);
        }



    }
}