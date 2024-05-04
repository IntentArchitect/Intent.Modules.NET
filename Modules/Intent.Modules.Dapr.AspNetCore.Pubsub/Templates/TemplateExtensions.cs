using System.Collections.Generic;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprEventHandlerController;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventBusImplementation;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandlerImplementation;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDaprEventHandlerControllerName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprEventHandlerControllerTemplate.TemplateId);
        }

        public static string GetEventBusImplementationName(this IIntentTemplate template)
        {
            return template.GetTypeName(EventBusImplementationTemplate.TemplateId);
        }

        public static string GetEventHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(EventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetEventHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(EventHandlerTemplate.TemplateId, model);
        }

        public static string GetEventHandlerImplementationName<T>(this IIntentTemplate<T> template) where T : MessageModel
        {
            return template.GetTypeName(EventHandlerImplementationTemplate.TemplateId, template.Model);
        }

        public static string GetEventHandlerImplementationName(this IIntentTemplate template, MessageModel model)
        {
            return template.GetTypeName(EventHandlerImplementationTemplate.TemplateId, model);
        }

        public static string GetEventInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(EventInterfaceTemplate.TemplateId);
        }

    }
}