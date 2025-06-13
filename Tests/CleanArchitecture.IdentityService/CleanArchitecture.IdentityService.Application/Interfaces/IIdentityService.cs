using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<string> ConfirmEmail(string userId, string code, string? changedEmail, CancellationToken cancellationToken = default);
        Task ForgotPassword(ForgotPasswordRequestDto resetRequest, CancellationToken cancellationToken = default);
        Task<InfoResponseDto> GetInfo(CancellationToken cancellationToken = default);
        Task<AccessTokenResponseDto> Login(LoginRequestDto login, bool? useCookies, bool? useSessionCookies, CancellationToken cancellationToken = default);
        Task<AccessTokenResponseDto> Refresh(RefreshRequestDto refreshRequest, CancellationToken cancellationToken = default);
        Task Register(RegisterRequestDto register, CancellationToken cancellationToken = default);
        Task ResendConfirmationEmail(ResendConfirmationEmailRequestDto resendRequest, CancellationToken cancellationToken = default);
        Task ResetPassword(ResetPasswordRequestDto resetRequest, CancellationToken cancellationToken = default);
        Task<InfoResponseDto> UpdateInfo(InfoRequestDto infoRequest, CancellationToken cancellationToken = default);
        Task<TwoFactorResponseDto> UpdateTwoFactor(TwoFactorRequestDto tfaRequest, CancellationToken cancellationToken = default);
    }
}