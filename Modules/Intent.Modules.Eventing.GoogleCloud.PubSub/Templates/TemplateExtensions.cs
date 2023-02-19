using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ConfigurationTemplates.GoogleCloudPubSubConfiguration;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.GenericIntegrationEventHandlerImplementation;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.GoogleCloudPubSubSubscriberBackgroundService;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.GoogleCloudResourceManager;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.GoogleEventBusSubscriptionManager;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.GoogleEventBusTopicEventManager;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.GooglePubSubEventBus;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.IntegrationEventHandlerImplementation;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.InterfaceTemplates.CloudResourceManagerInterface;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.InterfaceTemplates.EventBusSubscriptionManagerInterface;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.InterfaceTemplates.EventBusTopicEventManagerInterface;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.MessageTemplates.GenericMessage;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCloudResourceManagerInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CloudResourceManagerInterfaceTemplate.TemplateId);
        }

        public static string GetEventBusSubscriptionManagerInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(EventBusSubscriptionManagerInterfaceTemplate.TemplateId);
        }

        public static string GetEventBusTopicEventManagerInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(EventBusTopicEventManagerInterfaceTemplate.TemplateId);
        }

        public static string GetGenericIntegrationEventHandlerImplementationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GenericIntegrationEventHandlerImplementationTemplate.TemplateId);
        }
        public static string GetGenericMessageName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GenericMessageTemplate.TemplateId);
        }

        public static string GetGoogleCloudPubSubConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GoogleCloudPubSubConfigurationTemplate.TemplateId);
        }

        public static string GetGoogleCloudPubSubSubscriberBackgroundServiceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GoogleCloudPubSubSubscriberBackgroundServiceTemplate.TemplateId);
        }

        public static string GetGoogleCloudResourceManagerName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GoogleCloudResourceManagerTemplate.TemplateId);
        }

        public static string GetGoogleEventBusSubscriptionManagerName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GoogleEventBusSubscriptionManagerTemplate.TemplateId);
        }

        public static string GetGoogleEventBusTopicEventManagerName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GoogleEventBusTopicEventManagerTemplate.TemplateId);
        }

        public static string GetGooglePubSubEventBusName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(GooglePubSubEventBusTemplate.TemplateId);
        }

        public static string GetIntegrationEventHandlerImplementationName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageSubscribeAssocationTargetEndModel
        {
            return template.GetTypeName(IntegrationEventHandlerImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerImplementationName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageSubscribeAssocationTargetEndModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerImplementationTemplate.TemplateId, model);
        }

    }
}