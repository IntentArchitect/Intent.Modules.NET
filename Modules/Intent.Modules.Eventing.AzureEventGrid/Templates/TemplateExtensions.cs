using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridConfiguration;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridEventBus;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridMessageDispatcher;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridMessageDispatcherInterface;
using Intent.Modules.Eventing.AzureEventGrid.Templates.IntegrationEventHandler;
using Intent.Modules.Eventing.AzureEventGrid.Templates.PublisherOptions;
using Intent.Modules.Eventing.AzureEventGrid.Templates.SubscriptionOptions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureEventGridConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridConfigurationTemplate.TemplateId);
        }

        public static string GetAzureEventGridEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridEventBusTemplate.TemplateId);
        }

        public static string GetAzureEventGridMessageDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridMessageDispatcherTemplate.TemplateId);
        }

        public static string GetAzureEventGridMessageDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridMessageDispatcherInterfaceTemplate.TemplateId);
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