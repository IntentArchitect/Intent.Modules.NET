using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandlerImplementation;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitEventBus;
using Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates
{
    public static class TemplateExtensions
    {

        public static string GetIntegrationEventHandlerImplementationName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageSubscribeAssocationTargetEndModel
        {
            return template.GetTypeName(IntegrationEventHandlerImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerImplementationName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageSubscribeAssocationTargetEndModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerImplementationTemplate.TemplateId, model);
        }

        public static string GetMassTransitConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MassTransitConfigurationTemplate.TemplateId);
        }

        public static string GetMassTransitEventBusName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MassTransitEventBusTemplate.TemplateId);
        }

        public static string GetWrapperConsumerName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(WrapperConsumerTemplate.TemplateId);
        }

    }
}