using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates
{
    public static class TemplateExtensions
    {
        public static string GetEventBusInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(EventBusInterfaceTemplate.TemplateId);
        }

        public static string GetIntegrationEventHandlerInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(IntegrationEventHandlerInterfaceTemplate.TemplateId);
        }

        public static string GetIntegrationEventMessageName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageModel
        {
            return template.GetTypeName(IntegrationEventMessageTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventMessageName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageModel model)
        {
            return template.GetTypeName(IntegrationEventMessageTemplate.TemplateId, model);
        }

    }
}