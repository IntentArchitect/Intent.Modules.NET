using CleanArchitecture.TestApplication.BlazorClient.HttpClients.AccountService;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.AccountController.AccountServiceInterfaceTemplate", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients
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