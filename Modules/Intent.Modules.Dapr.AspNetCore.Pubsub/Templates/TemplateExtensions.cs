using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprConfiguration;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprEventHandlerController;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprMessageBus;
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

        public static string GetDaprEventHandlerControllerName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprEventHandlerControllerTemplate.TemplateId);
        }

        public static string GetDaprMessageBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprMessageBusTemplate.TemplateId);
        }

        public static string GetEventInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(EventInterfaceTemplate.TemplateId);
        }

    }
}