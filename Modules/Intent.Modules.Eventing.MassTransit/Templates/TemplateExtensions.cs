using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.FinbuckleConsumingFilter;
using Intent.Modules.Eventing.MassTransit.Templates.FinbuckleMessageHeaderStrategy;
using Intent.Modules.Eventing.MassTransit.Templates.FinbucklePublishingFilter;
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
        public static string GetIntegrationEventHandlerImplementationName<T>(this IIntentTemplate<T> template) where T : Intent.Modelers.Eventing.Api.MessageSubscribeAssocationTargetEndModel
        {
            return template.GetTypeName(IntegrationEventHandlerImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerImplementationName(this IIntentTemplate template, Intent.Modelers.Eventing.Api.MessageSubscribeAssocationTargetEndModel model)
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

        public static string GetWrapperConsumerName(this IIntentTemplate template)
        {
            return template.GetTypeName(WrapperConsumerTemplate.TemplateId);
        }

    }
}