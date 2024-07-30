using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using OpenApiImporterTest.Application.Accounts;
using OpenApiImporterTest.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace OpenApiImporterTest.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class AccountService : IAccountService
    {
        [IntentManaged(Mode.Merge)]
        public AccountService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateAccount(CreateAccountCommand dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateAccount (AccountService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<AccountDto>> GetAccounts(CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetAccounts (AccountService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DeleteAccount(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DeleteAccount (AccountService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateAccount(UpdateAccountCommand dto, Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateAccount (AccountService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<AccountDto> GetAccount(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetAccount (AccountService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}