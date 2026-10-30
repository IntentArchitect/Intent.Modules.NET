using System.Collections.Generic;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DefaultDomainEventHandler;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventHandler;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventNotification;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventNotificationFactory;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventService;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.TypedDomainEventNotification;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDefaultDomainEventHandlerName<T>(this IIntentTemplate<T> template) where T : DomainEventModel
        {
            return template.GetTypeName(DefaultDomainEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetDefaultDomainEventHandlerName(this IIntentTemplate template, DomainEventModel model)
        {
            return template.GetTypeName(DefaultDomainEventHandlerTemplate.TemplateId, model);
        }

        public static string GetDomainEventHandlerName<T>(this IIntentTemplate<T> template) where T : DomainEventHandlerModel
        {
            return template.GetTypeName(DomainEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEventHandlerName(this IIntentTemplate template, DomainEventHandlerModel model)
        {
            return template.GetTypeName(DomainEventHandlerTemplate.TemplateId, model);
        }
        public static string GetDomainEventNotificationName(this IIntentTemplate template)
        {
            return template.GetTypeName(DomainEventNotificationTemplate.TemplateId);
        }

        public static string GetDomainEventNotificationFactoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(DomainEventNotificationFactoryTemplate.TemplateId);
        }

        public static string GetDomainEventServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DomainEventServiceTemplate.TemplateId);
        }

        public static string GetTypedDomainEventNotificationName<T>(this IIntentTemplate<T> template) where T : DomainEventModel
        {
            return template.GetTypeName(TypedDomainEventNotificationTemplate.TemplateId, template.Model);
        }

        public static string GetTypedDomainEventNotificationName(this IIntentTemplate template, DomainEventModel model)
        {
            return template.GetTypeName(TypedDomainEventNotificationTemplate.TemplateId, model);
        }

    }
}