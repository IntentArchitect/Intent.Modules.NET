using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.ObjectMapping.Templates.MappingExtensions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.ObjectMapping.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMappingExtensionsName<T>(this IIntentTemplate<T> template) where T : DTOModel
        {
            return template.GetTypeName(MappingExtensionsTemplate.TemplateId, template.Model);
        }

        public static string GetMappingExtensionsName(this IIntentTemplate template, DTOModel model)
        {
            return template.GetTypeName(MappingExtensionsTemplate.TemplateId, model);
        }

    }
}