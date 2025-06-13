using CleanArchitecture.IdentityService.Application.Identity;
using CleanArchitecture.IdentityService.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class IdentityService : IIdentityService
    {
        private readonly IIdentityServiceManager _identityServiceManager;

        [IntentManaged(Mode.Merge)]
        public IdentityService(IIdentityServiceManager identityServiceManager)
        {
            _identityServiceManager = identityServiceManager;
        }

        [IntentManaged(Mode.Fully)]
        public async Task<string> ConfirmEmail(
            string userId,
            string code,
            string? changedEmail,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.ConfirmEmail(userId, code, changedEmail);
        }

        [IntentManaged(Mode.Fully)]
        public async Task ForgotPassword(
            ForgotPasswordRequestDto resetRequest,
            CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.ForgotPassword(resetRequest);
        }

        [IntentManaged(Mode.Fully)]
        public async Task<InfoResponseDto> GetInfo(CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.GetInfo();
        }

        [IntentManaged(Mode.Fully)]
        public async Task<AccessTokenResponseDto> Login(
            LoginRequestDto login,
            bool? useCookies,
            bool? useSessionCookies,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.Login(login, useCookies, useSessionCookies);
        }

        [IntentManaged(Mode.Fully)]
        public async Task<AccessTokenResponseDto> Refresh(
            RefreshRequestDto refreshRequest,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.Refresh(refreshRequest);
        }

        [IntentManaged(Mode.Fully)]
        public async Task Register(RegisterRequestDto register, CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.Register(register);
        }

        [IntentManaged(Mode.Fully)]
        public async Task ResendConfirmationEmail(
            ResendConfirmationEmailRequestDto resendRequest,
            CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.ResendConfirmationEmail(resendRequest);
        }

        [IntentManaged(Mode.Fully)]
        public async Task ResetPassword(
            ResetPasswordRequestDto resetRequest,
            CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.ResetPassword(resetRequest);
        }

        [IntentManaged(Mode.Fully)]
        public async Task<InfoResponseDto> UpdateInfo(
            InfoRequestDto infoRequest,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.UpdateInfo(infoRequest);
        }

        [IntentManaged(Mode.Fully)]
        public async Task<TwoFactorResponseDto> UpdateTwoFactor(
            TwoFactorRequestDto tfaRequest,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.UpdateTwoFactor(tfaRequest);
        }
    }
}