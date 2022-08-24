using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.Consumer;
using Intent.Modules.Eventing.MassTransit.Templates.EventHandlerImplementation;
using Intent.Modules.Eventing.MassTransit.Templates.EventHandlerInterface;
using Intent.Modules.Eventing.MassTransit.Templates.EventMessage;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates
{
    public static class TemplateExtensions
    {
        public static string GetConsumerName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageHandlerModel
        {
            return template.GetTypeName(ConsumerTemplate.TemplateId, template.Model);
        }

        public static string GetConsumerName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageHandlerModel model)
        {
            return template.GetTypeName(ConsumerTemplate.TemplateId, model);
        }

        public static string GetEventHandlerImplementationName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageHandlerModel
        {
            return template.GetTypeName(EventHandlerImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetEventHandlerImplementationName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageHandlerModel model)
        {
            return template.GetTypeName(EventHandlerImplementationTemplate.TemplateId, model);
        }

        public static string GetEventHandlerInterfaceName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageHandlerModel
        {
            return template.GetTypeName(EventHandlerInterfaceTemplate.TemplateId, template.Model);
        }

        public static string GetEventHandlerInterfaceName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageHandlerModel model)
        {
            return template.GetTypeName(EventHandlerInterfaceTemplate.TemplateId, model);
        }

        public static string GetEventMessageName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageModel
        {
            return template.GetTypeName(EventMessageTemplate.TemplateId, template.Model);
        }

        public static string GetEventMessageName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageModel model)
        {
            return template.GetTypeName(EventMessageTemplate.TemplateId, model);
        }

        public static string GetMassTransitConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MassTransitConfigurationTemplate.TemplateId);
        }

    }
}