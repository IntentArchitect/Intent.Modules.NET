using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.AuthServiceInterfaceTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Jwt.Common
{
    public interface IAuthService
    {
        Task Login(string email, string password, bool rememberMe, string returnUrl);
        Task<string> ConfirmEmail(string? userId, string? code);
        Task ForgotPassword(string email);
        Task<IEnumerable<IdentityError>> Register(string email, string password, string returnUrl);
        Task ResendEmailConfirmation(string email);
        Task ResetPassword(string email, string code, string password);
    }
}