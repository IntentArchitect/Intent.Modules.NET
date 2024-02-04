using System.Collections.Generic;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
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

        public static string GetIntegrationCommandName<T>(this IIntentTemplate<T> template) where T : IntegrationCommandModel
        {
            return template.GetTypeName(IntegrationCommandTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationCommandName(this IIntentTemplate template, IntegrationCommandModel model)
        {
            return template.GetTypeName(IntegrationCommandTemplate.TemplateId, model);
        }

        public static string GetIntegrationEventDtoName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.EventingDTOModel
        {
            return template.GetTypeName(IntegrationEventDtoTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventDtoName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.EventingDTOModel model)
        {
            return template.GetTypeName(IntegrationEventDtoTemplate.TemplateId, model);
        }

        public static string GetIntegrationEventEnumName<T>(this IntentTemplateBase<T> template) where T : Intent.Modules.Common.Types.Api.EnumModel
        {
            return template.GetTypeName(IntegrationEventEnumTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventEnumName(this IntentTemplateBase template, Intent.Modules.Common.Types.Api.EnumModel model)
        {
            return template.GetTypeName(IntegrationEventEnumTemplate.TemplateId, model);
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