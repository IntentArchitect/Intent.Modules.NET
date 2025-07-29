using Intent.Modules.Application.Dtos.Pagination.Templates.CursorPagedResult;
using Intent.Modules.Application.Dtos.Pagination.Templates.CursorPagedResultMappingExtensions;
using Intent.Modules.Application.Dtos.Pagination.Templates.PagedResult;
using Intent.Modules.Application.Dtos.Pagination.Templates.PagedResultMappingExtensions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates
{
    public static class TemplateExtensions
    {
        public static string GetPagedResultName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(PagedResultTemplate.TemplateId);
        }

        public static string GetCursorPagedResultName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CursorPagedResultTemplate.TemplateId);
        }

        public static string GetCursorPagedResultMappingExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(CursorPagedResultMappingExtensionsTemplate.TemplateId);
        }

        public static string GetPagedResultMappingExtensionsName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(PagedResultMappingExtensionsTemplate.TemplateId);
        }

    }
}