using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.AuthServiceInterfaceTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Common
{
    public interface IAuthService
    {
        Task Login(string email, string password, bool rememberMe, string returnUrl);
        Task<string> ConfirmEmail(string? userId, string? code);
        Task ForgotPassword(string email);
        Task Register(string email, string password, string returnUrl);
        Task ResendEmailConfirmation(string email);
        Task ResetPassword(string email, string code, string password);
    }
}