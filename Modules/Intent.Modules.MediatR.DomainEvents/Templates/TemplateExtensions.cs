using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.MediatR.DomainEvents.Templates.AggregateManager;
using Intent.Modules.MediatR.DomainEvents.Templates.DefaultDomainEventHandler;
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
        public static string GetAggregateManagerName<T>(this IIntentTemplate<T> template) where T : ClassModel
        {
            return template.GetTypeName(AggregateManagerTemplate.TemplateId, template.Model);
        }

        public static string GetAggregateManagerName(this IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName(AggregateManagerTemplate.TemplateId, model);
        }

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

        public static string GetDomainEventServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DomainEventServiceTemplate.TemplateId);
        }

    }
}