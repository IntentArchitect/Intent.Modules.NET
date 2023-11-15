using Intent.RoslynWeaver.Attributes;
using MinimalHostingModel.BlazorClient.HttpClients.AccountService;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.AccountController.AccountServiceInterfaceTemplate", Version = "1.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients
{
    public interface IAccountService
    {
        Task Register(RegisterDto command, CancellationToken cancellationToken = default);
        Task<TokenResultDto> Login(LoginDto command, CancellationToken cancellationToken = default);
        Task<TokenResultDto> RefreshToken(string authenticationToken, string refreshToken, CancellationToken cancellationToken = default);
        Task ConfirmEmail(ConfirmEmailDto command, CancellationToken cancellationToken = default);
        Task Logout(CancellationToken cancellationToken = default);
    }
}