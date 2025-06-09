using CleanArchitecture.IdentityService.Application.Identity;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<string> ConfirmEmailAsync(string userId, string code, string? changedEmail, CancellationToken cancellationToken = default);
        Task ForgotPasswordAsync(ForgotPasswordRequestDto resetRequest, CancellationToken cancellationToken = default);
        Task<InfoResponseDto> GetInfoAsync(CancellationToken cancellationToken = default);
        Task<AccessTokenResponseDto> LoginAsync(LoginRequestDto login, bool? useCookies, bool? useSessionCookies, CancellationToken cancellationToken = default);
        Task<AccessTokenResponseDto> RefreshAsync(RefreshRequestDto refreshRequest, CancellationToken cancellationToken = default);
        Task RegisterAsync(RegisterRequestDto register, CancellationToken cancellationToken = default);
        Task ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto resendRequest, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(ResetPasswordRequestDto resetRequest, CancellationToken cancellationToken = default);
        Task<InfoResponseDto> UpdateInfoAsync(InfoRequestDto infoRequest, CancellationToken cancellationToken = default);
        Task<TwoFactorResponseDto> UpdateTwoFactorAsync(TwoFactorRequestDto tfaRequest, CancellationToken cancellationToken = default);
    }
}