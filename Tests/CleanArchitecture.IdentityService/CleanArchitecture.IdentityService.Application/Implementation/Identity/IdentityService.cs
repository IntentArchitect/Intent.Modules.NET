using CleanArchitecture.IdentityService.Application.Identity;
using CleanArchitecture.IdentityService.Application.Interfaces;
using CleanArchitecture.IdentityService.Application.Interfaces.Identity;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Implementation.Identity
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> ConfirmEmailAsync(
            string userId,
            string code,
            string? changedEmail,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.ConfirmEmailAsync(userId, code, changedEmail);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ForgotPasswordAsync(
            ForgotPasswordRequestDto resetRequest,
            CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.ForgotPasswordAsync(resetRequest);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<InfoResponseDto> GetInfoAsync(CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.GetInfoAsync();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<AccessTokenResponseDto> LoginAsync(
            LoginRequestDto login,
            bool? useCookies,
            bool? useSessionCookies,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.LoginAsync(login, useCookies, useSessionCookies);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<AccessTokenResponseDto> RefreshAsync(
            RefreshRequestDto refreshRequest,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.RefreshAsync(refreshRequest);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task RegisterAsync(RegisterRequestDto register, CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.RegisterAsync(register);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ResendConfirmationEmailAsync(
            ResendConfirmationEmailRequestDto resendRequest,
            CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.ResendConfirmationEmailAsync(resendRequest);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task ResetPasswordAsync(
            ResetPasswordRequestDto resetRequest,
            CancellationToken cancellationToken = default)
        {
            await _identityServiceManager.ResetPasswordAsync(resetRequest);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<InfoResponseDto> UpdateInfoAsync(
            InfoRequestDto infoRequest,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.UpdateInfoAsync(infoRequest);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<TwoFactorResponseDto> UpdateTwoFactorAsync(
            TwoFactorRequestDto tfaRequest,
            CancellationToken cancellationToken = default)
        {
            return await _identityServiceManager.UpdateTwoFactorAsync(tfaRequest);
        }
    }
}