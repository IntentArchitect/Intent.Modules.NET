using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using OpenApiImporterTest.Application.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace OpenApiImporterTest.Application.Interfaces
{
    public interface IAccountService : IDisposable
    {
        Task<Guid> CreateAccount(CreateAccountCommand dto, CancellationToken cancellationToken = default);
        Task<List<AccountDto>> GetAccounts(CancellationToken cancellationToken = default);
        Task DeleteAccount(Guid id, CancellationToken cancellationToken = default);
        Task UpdateAccount(UpdateAccountCommand dto, Guid id, CancellationToken cancellationToken = default);
        Task<AccountDto> GetAccount(Guid id, CancellationToken cancellationToken = default);
    }
}