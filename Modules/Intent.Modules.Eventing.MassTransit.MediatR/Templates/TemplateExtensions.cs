using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.MediatR.Templates.MessageBusPublishBehaviour;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.MediatR.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMessageBusPublishBehaviourName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MessageBusPublishBehaviourTemplate.TemplateId);
        }

    }
}