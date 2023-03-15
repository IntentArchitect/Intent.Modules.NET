using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprEventHandlerController;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventBusImplementation;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDaprEventHandlerControllerName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DaprEventHandlerControllerTemplate.TemplateId);
        }

        public static string GetEventBusImplementationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(EventBusImplementationTemplate.TemplateId);
        }

        public static string GetEventHandlerName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageModel
        {
            return template.GetTypeName(EventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetEventHandlerName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageModel model)
        {
            return template.GetTypeName(EventHandlerTemplate.TemplateId, model);
        }

        public static string GetEventInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(EventInterfaceTemplate.TemplateId);
        }

    }
}