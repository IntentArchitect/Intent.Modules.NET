using System.Collections.Generic;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.DomainMapping.Templates.DtoExtensions;
using Intent.Modules.Eventing.Contracts.DomainMapping.Templates.MessageExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.DomainMapping.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDtoExtensionsName<T>(this IIntentTemplate<T> template) where T : EventingDTOModel
        {
            return template.GetTypeName(DtoExtensionsTemplate.TemplateId, template.Model);
        }

        public static string GetDtoExtensionsName(this IIntentTemplate template, EventingDTOModel model)
        {
            return template.GetTypeName(DtoExtensionsTemplate.TemplateId, model);
        }

        public static string GetMessageExtensionsName<T>(this IIntentTemplate<T> template) where T : MessageModel
        {
            return template.GetTypeName(MessageExtensionsTemplate.TemplateId, template.Model);
        }

        public static string GetMessageExtensionsName(this IIntentTemplate template, MessageModel model)
        {
            return template.GetTypeName(MessageExtensionsTemplate.TemplateId, model);
        }

    }
}