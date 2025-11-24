using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprConfiguration;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprEventBus;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprEventHandlerController;
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
        public static string GetDaprConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprConfigurationTemplate.TemplateId);
        }

        public static string GetDaprEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprEventBusTemplate.TemplateId);
        }

        public static string GetDaprEventHandlerControllerName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprEventHandlerControllerTemplate.TemplateId);
        }

        public static string GetEventHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(EventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetEventHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(EventHandlerTemplate.TemplateId, model);
        }

        public static string GetEventInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(EventInterfaceTemplate.TemplateId);
        }

    }
}