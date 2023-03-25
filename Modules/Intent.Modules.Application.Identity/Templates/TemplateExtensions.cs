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
        public static string GetApplicationSecurityConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ApplicationSecurityConfigurationTemplate.TemplateId);
        }
        public static string GetAuthorizeAttributeName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AuthorizeAttributeTemplate.TemplateId);
        }

        public static string GetCurrentUserServiceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CurrentUserServiceTemplate.TemplateId);
        }

        public static string GetCurrentUserServiceInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CurrentUserServiceInterfaceTemplate.TemplateId);
        }

        public static string GetForbiddenAccessExceptionName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ForbiddenAccessExceptionTemplate.TemplateId);
        }

        public static string GetResultModelName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ResultModelTemplate.TemplateId);
        }

    }
}