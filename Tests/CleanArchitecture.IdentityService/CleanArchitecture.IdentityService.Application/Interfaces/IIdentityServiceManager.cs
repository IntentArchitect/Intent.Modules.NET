using System.Security.Claims;
using CleanArchitecture.IdentityService.Application.Identity;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.IdentityServiceManagerInterface", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Interfaces
{
    public interface IIdentityServiceManager
    {
        Task RegisterAsync(RegisterRequestDto registration);
        Task<AccessTokenResponseDto> LoginAsync(LoginRequestDto login, bool? useCookies, bool? useSessionCookies);
        Task<AccessTokenResponseDto> RefreshAsync(RefreshRequestDto refreshRequest);
        Task<string> ConfirmEmailAsync(string userId, string code, string? changedEmail);
        Task<bool> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto resendRequest);
        Task ForgotPasswordAsync(ForgotPasswordRequestDto resetRequest);
        Task ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest);
        Task<TwoFactorResponseDto> UpdateTwoFactorAsync(TwoFactorRequestDto tfaRequest);
        Task<InfoResponseDto> GetInfoAsync();
        Task<InfoResponseDto> UpdateInfoAsync(InfoRequestDto infoRequest);
    }
}