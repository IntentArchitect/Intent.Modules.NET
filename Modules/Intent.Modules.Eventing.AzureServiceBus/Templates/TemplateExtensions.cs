using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusConfiguration;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusEventBus;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusMessageDispatcher;
using Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusMessageDispatcherInterface;
using Intent.Modules.Eventing.AzureServiceBus.Templates.IntegrationEventHandler;
using Intent.Modules.Eventing.AzureServiceBus.Templates.PublisherOptions;
using Intent.Modules.Eventing.AzureServiceBus.Templates.SubscriptionOptions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureServiceBusConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusConfigurationTemplate.TemplateId);
        }

        public static string GetAzureServiceBusEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusEventBusTemplate.TemplateId);
        }

        public static string GetAzureServiceBusMessageDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusMessageDispatcherTemplate.TemplateId);
        }

        public static string GetAzureServiceBusMessageDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureServiceBusMessageDispatcherInterfaceTemplate.TemplateId);
        }

        public static string GetIntegrationEventHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, model);
        }

        public static string GetPublisherOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(PublisherOptionsTemplate.TemplateId);
        }

        public static string GetSubscriptionOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(SubscriptionOptionsTemplate.TemplateId);
        }

    }
}