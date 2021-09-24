using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.DomainEvents.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDomainEventName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Events.Api.DomainEventModel
        {
            return template.GetTypeName(DomainEventTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEventName(this IntentTemplateBase template, Intent.Modelers.Domain.Events.Api.DomainEventModel model)
        {
            return template.GetTypeName(DomainEventTemplate.TemplateId, model);
        }

        public static string GetDomainEventBaseName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DomainEventBaseTemplate.TemplateId);
        }

        public static string GetDomainEventServiceInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId);
        }

        public static string GetHasDomainEventInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId);
        }

    }
}