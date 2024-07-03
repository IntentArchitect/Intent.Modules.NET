using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.AutoMapper.Templates.MappingExtensions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.Templates
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

        [IntentIgnore]
        // Kept for backwards compatibility
        public static string GetMappingExtensionsName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.DTOModel
        {
            return template.GetTypeName(MappingExtensionsTemplate.TemplateId, template.Model);
        }

        [IntentIgnore]
        // Kept for backwards compatibility
        public static string GetMappingExtensionsName(this IntentTemplateBase template, Intent.Modelers.Services.Api.DTOModel model)
        {
            return template.GetTypeName(MappingExtensionsTemplate.TemplateId, model);
        }

    }
}