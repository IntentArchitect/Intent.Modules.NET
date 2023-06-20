using System.Collections.Generic;
using Intent.Modules.Application.Identity.Templates.ApplicationSecurityConfiguration;
using Intent.Modules.Application.Identity.Templates.AuthorizeAttribute;
using Intent.Modules.Application.Identity.Templates.CurrentUserService;
using Intent.Modules.Application.Identity.Templates.CurrentUserServiceInterface;
using Intent.Modules.Application.Identity.Templates.ForbiddenAccessException;
using Intent.Modules.Application.Identity.Templates.ResultModel;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Templates
{
    public static class TemplateExtensions
    {
        public static string GetApplicationSecurityConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(ApplicationSecurityConfigurationTemplate.TemplateId);
        }
        public static string GetAuthorizeAttributeName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthorizeAttributeTemplate.TemplateId);
        }

        public static string GetCurrentUserServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CurrentUserServiceTemplate.TemplateId);
        }

        public static string GetCurrentUserServiceInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(CurrentUserServiceInterfaceTemplate.TemplateId);
        }

        public static string GetForbiddenAccessExceptionName(this IIntentTemplate template)
        {
            return template.GetTypeName(ForbiddenAccessExceptionTemplate.TemplateId);
        }

        public static string GetResultModelName(this IIntentTemplate template)
        {
            return template.GetTypeName(ResultModelTemplate.TemplateId);
        }

    }
}