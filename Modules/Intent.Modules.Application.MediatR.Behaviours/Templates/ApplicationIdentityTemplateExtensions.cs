using Intent.Modules.Application.Identity.Templates.AuthorizeAttribute;
using Intent.Modules.Application.Identity.Templates.CurrentUserServiceInterface;
using Intent.Modules.Application.Identity.Templates.ForbiddenAccessException;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.MediatR.Behaviours.Templates
{
    public static class ApplicationIdentityTemplateExtensions {

        public static string GetCurrentUserServiceInterface<TModel>(this CSharpTemplateBase<TModel> template)
        {
            return template.GetTypeName(CurrentUserServiceInterfaceTemplate.TemplateId);
        }

        public static string GetAuthorizationAttribute<TModel>(this CSharpTemplateBase<TModel> template)
        {
            return template.GetTypeName(AuthorizeAttributeTemplate.TemplateId);
        }

        public static string GetForbiddenAccessException<TModel>(this CSharpTemplateBase<TModel> template)
        {
            return template.GetTypeName(ForbiddenAccessExceptionTemplate.TemplateId);
        }
    }
}