using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.PTH.Application.AccountsServices;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.Interfaces
{
    public interface IAccountsService : IDisposable
    {
        Task<Guid> CreateAccountCommand(AccountsServices.CreateAccountCommand command, CancellationToken cancellationToken = default);
        Task UpdateAccountCommand(Guid id, AccountsServices.UpdateAccountCommand command, CancellationToken cancellationToken = default);
        Task<AccountDto> GetAccountByIdQuery(Guid id, CancellationToken cancellationToken = default);
        Task<List<AccountDto>> GetAccountsQuery(CancellationToken cancellationToken = default);
    }
}