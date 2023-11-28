using Intent.RoslynWeaver.Attributes;
using MinimalHostingModel.BlazorClient.HttpClients.AccountService;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.AccountController.AccountServiceInterfaceTemplate", Version = "1.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients
{
    public interface IAccountService
    {
        Task Register(RegisterDto dto, CancellationToken cancellationToken = default);
        Task<TokenResultDto> Login(LoginDto dto, CancellationToken cancellationToken = default);
        Task<TokenResultDto> Refresh(string refreshToken, CancellationToken cancellationToken = default);
        Task ConfirmEmail(ConfirmEmailDto dto, CancellationToken cancellationToken = default);
        Task Logout(CancellationToken cancellationToken = default);
    }
}