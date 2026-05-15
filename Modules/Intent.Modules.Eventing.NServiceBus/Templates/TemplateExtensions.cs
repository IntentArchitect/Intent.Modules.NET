using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusConsumer;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusPublisher;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates
{
    public static class TemplateExtensions
    {
        public static string GetNServiceBusConsumerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(NServiceBusConsumerTemplate.TemplateId, template.Model);
        }

        public static string GetNServiceBusConsumerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(NServiceBusConsumerTemplate.TemplateId, model);
        }

        public static string GetNServiceBusPublisherName(this IIntentTemplate template)
        {
            return template.GetTypeName(NServiceBusPublisherTemplate.TemplateId);
        }

    }
}