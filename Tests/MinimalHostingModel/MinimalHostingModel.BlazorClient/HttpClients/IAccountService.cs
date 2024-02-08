using Intent.RoslynWeaver.Attributes;
using MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients
{
    public interface IAccountService : IDisposable
    {
        Task RegisterAsync(RegisterDto input, CancellationToken cancellationToken = default);
        Task<TokenResultDto> LoginAsync(LoginDto input, CancellationToken cancellationToken = default);
        Task<TokenResultDto> RefreshAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
        Task ConfirmEmailAsync(ConfirmEmailDto input, CancellationToken cancellationToken = default);
        Task ForgotPasswordAsync(ForgotPasswordDto resetRequest, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(ResetPasswordDto resetRequest, CancellationToken cancellationToken = default);
        Task<InfoResponseDto> GetInfoAsync(CancellationToken cancellationToken = default);
        Task PostInfoAsync(UpdateInfoDto infoRequest, CancellationToken cancellationToken = default);
        Task LogoutAsync(CancellationToken cancellationToken = default);
    }
}