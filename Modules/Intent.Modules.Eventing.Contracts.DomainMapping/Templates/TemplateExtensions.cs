using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.DomainMapping.Templates.MessageExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.DomainMapping.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMessageExtensionsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Eventing.Api.MessageModel
        {
            return template.GetTypeName(MessageExtensionsTemplate.TemplateId, template.Model);
        }

        public static string GetMessageExtensionsName(this IntentTemplateBase template, Intent.Modelers.Eventing.Api.MessageModel model)
        {
            return template.GetTypeName(MessageExtensionsTemplate.TemplateId, model);
        }

    }
}