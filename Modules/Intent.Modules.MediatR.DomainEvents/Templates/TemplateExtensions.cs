using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventHandler;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventService;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDomainEventHandlerName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Events.Api.DomainEventModel
        {
            return template.GetTypeName(DomainEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEventHandlerName(this IntentTemplateBase template, Intent.Modelers.Domain.Events.Api.DomainEventModel model)
        {
            return template.GetTypeName(DomainEventHandlerTemplate.TemplateId, model);
        }

        public static string GetDomainEventNotificationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DomainEventNotificationTemplate.TemplateId);
        }

        public static string GetDomainEventServiceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DomainEventServiceTemplate.TemplateId);
        }

    }
}