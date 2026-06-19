using System.Collections.Generic;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Application.Wolverine.DomainEvents.Templates.DomainEventHandler;
using Intent.Modules.Application.Wolverine.DomainEvents.Templates.DomainEventService;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.DomainEvents.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDomainEventHandlerName<T>(this IIntentTemplate<T> template) where T : DomainEventHandlerModel
        {
            return template.GetTypeName(DomainEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetDomainEventHandlerName(this IIntentTemplate template, DomainEventHandlerModel model)
        {
            return template.GetTypeName(DomainEventHandlerTemplate.TemplateId, model);
        }
        public static string GetDomainEventServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DomainEventServiceTemplate.TemplateId);
        }

    }
}