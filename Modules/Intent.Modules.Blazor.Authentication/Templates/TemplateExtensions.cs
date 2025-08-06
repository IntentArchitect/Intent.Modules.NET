using System.Collections.Generic;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.PersistentAuthenticationStateProvider;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.RedirectToLogin;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.ServerAuthorizationMessageHandler;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.UserInfo;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AccountLayout;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationDbContext;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AspNetCoreIdentityAuthServiceConcrete;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ConfirmEmail;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ForgotPassword;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityComponentsEndpointRouteBuilderExtensions;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityNoOpEmailSender;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRevalidatingAuthenticationStateProvider;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityUserAccessor;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.JwtAuthServiceConcrete;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.Login;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.OidcAuthenticationOptions;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.OidcAuthServiceConcrete;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.PersistingRevalidatingAuthenticationStateProvider;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.PersistingServerAuthenticationStateProvider;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.Register;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ResendEmailConfirmation;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ResetPassword;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.SetUserContextInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates
{
    public static class TemplateExtensions
    {
        public static string GetPersistentAuthenticationStateProviderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(PersistentAuthenticationStateProviderTemplate.TemplateId);
        }
        public static string GetUserInfoTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(UserInfoTemplate.TemplateId);
        }
        public static string GetApplicationDbContextTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ApplicationDbContextTemplate.TemplateId);
        }

        public static string GetApplicationUserTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ApplicationUserTemplate.TemplateId);
        }

        public static string GetAspNetCoreIdentityAuthServiceConcreteTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(AspNetCoreIdentityAuthServiceConcreteTemplate.TemplateId);
        }

        public static string GetAuthServiceInterfaceTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthServiceInterfaceTemplate.TemplateId);
        }

        public static string GetIdentityComponentsEndpointRouteBuilderExtensionsTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityComponentsEndpointRouteBuilderExtensionsTemplate.TemplateId);
        }

        public static string GetIdentityNoOpEmailSenderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityNoOpEmailSenderTemplate.TemplateId);
        }

        public static string GetIdentityRedirectManagerTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityRedirectManagerTemplate.TemplateId);
        }

        public static string GetIdentityRevalidatingAuthenticationStateProviderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityRevalidatingAuthenticationStateProviderTemplate.TemplateId);
        }

        public static string GetIdentityUserAccessorTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityUserAccessorTemplate.TemplateId);
        }

        public static string GetJwtAuthServiceConcreteTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(JwtAuthServiceConcreteTemplate.TemplateId);
        }

        public static string GetOidcAuthenticationOptionsTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(OidcAuthenticationOptionsTemplate.TemplateId);
        }

        public static string GetOidcAuthServiceConcreteTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(OidcAuthServiceConcreteTemplate.TemplateId);
        }

        public static string GetPersistingRevalidatingAuthenticationStateProviderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(PersistingRevalidatingAuthenticationStateProviderTemplate.TemplateId);
        }

        public static string GetPersistingServerAuthenticationStateProviderTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(PersistingServerAuthenticationStateProviderTemplate.TemplateId);
        }

        public static string GetSetUserContextInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(SetUserContextInterfaceTemplate.TemplateId);
        }

        public static string GetServerAuthorizationMessageHandlerTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ServerAuthorizationMessageHandlerTemplate.TemplateId);
        }

        public static string GetAccountLayoutTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(AccountLayoutTemplate.TemplateId);
        }

        public static string GetConfirmEmailTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ConfirmEmailTemplate.TemplateId);
        }

        public static string GetForgotPasswordTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ForgotPasswordTemplate.TemplateId);
        }

        public static string GetLoginTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(LoginTemplate.TemplateId);
        }

        public static string GetRedirectToLoginTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(RedirectToLoginTemplate.TemplateId);
        }

        public static string GetRegisterTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(RegisterTemplate.TemplateId);
        }

        public static string GetResendEmailConfirmationTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ResendEmailConfirmationTemplate.TemplateId);
        }

        public static string GetResetPasswordTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ResetPasswordTemplate.TemplateId);
        }

    }
}