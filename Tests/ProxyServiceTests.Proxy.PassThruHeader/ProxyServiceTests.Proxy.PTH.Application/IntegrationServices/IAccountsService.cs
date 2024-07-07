using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.IntegrationServices
{
    public interface IAccountsService : IDisposable
    {
        Task<Guid> CreateAccountAsync(CreateAccountCommand command, CancellationToken cancellationToken = default);
        Task UpdateAccountAsync(Guid id, UpdateAccountCommand command, CancellationToken cancellationToken = default);
        Task<AccountDto> GetAccountByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken = default);
    }
}