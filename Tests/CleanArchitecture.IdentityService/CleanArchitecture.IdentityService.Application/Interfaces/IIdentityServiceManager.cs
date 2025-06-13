using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.IdentityServiceManagerInterface", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Interfaces
{
    public interface IIdentityServiceManager
    {
        Task Register(RegisterRequestDto registration);
        Task<AccessTokenResponseDto> Login(LoginRequestDto login, bool? useCookies, bool? useSessionCookies);
        Task<AccessTokenResponseDto> Refresh(RefreshRequestDto refreshRequest);
        Task<string> ConfirmEmail(string userId, string code, string? changedEmail);
        Task<bool> ResendConfirmationEmail(ResendConfirmationEmailRequestDto resendRequest);
        Task ForgotPassword(ForgotPasswordRequestDto resetRequest);
        Task ResetPassword(ResetPasswordRequestDto resetPasswordRequest);
        Task<TwoFactorResponseDto> UpdateTwoFactor(TwoFactorRequestDto tfaRequest);
        Task<InfoResponseDto> GetInfo();
        Task<InfoResponseDto> UpdateInfo(InfoRequestDto infoRequest);
    }
}