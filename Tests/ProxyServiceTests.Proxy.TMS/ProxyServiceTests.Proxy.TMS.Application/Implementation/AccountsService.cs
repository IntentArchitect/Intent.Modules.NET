using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.TMS.Application.AccountsServices;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices;
using ProxyServiceTests.Proxy.TMS.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;
using ProxyServiceTests.Proxy.TMS.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class AccountsService : Interfaces.IAccountsService
    {
        private readonly IntegrationServices.IAccountsService _ntegrationServicesIAccountsService;

        [IntentManaged(Mode.Merge)]
        public AccountsService(IntegrationServices.IAccountsService ntegrationServicesIAccountsService)
        {
            _ntegrationServicesIAccountsService = ntegrationServicesIAccountsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateAccountCommand(
            AccountsServices.CreateAccountCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _ntegrationServicesIAccountsService.CreateAccountAsync(new IntegrationServices.Contracts.OriginalServices.Services.Accounts.CreateAccountCommand
            {
                Number = command.Number,
                Amount = new IntegrationServices.Contracts.OriginalServices.Services.Accounts.CreateAccountMoneyDto
                {
                    Amount = command.Amount.Amount,
                    Currency = command.Amount.Currency
                },
                ClientId = command.ClientId
            }, cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateAccountCommand(
            Guid id,
            AccountsServices.UpdateAccountCommand command,
            CancellationToken cancellationToken = default)
        {
            await _ntegrationServicesIAccountsService.UpdateAccountAsync(id, new IntegrationServices.Contracts.OriginalServices.Services.Accounts.UpdateAccountCommand
            {
                Number = command.Number,
                Amount = new IntegrationServices.Contracts.OriginalServices.Services.Accounts.UpdateAccountMoneyDto
                {
                    Amount = command.Amount.Amount,
                    Currency = command.Amount.Currency
                },
                ClientId = command.ClientId,
                Id = command.Id
            }, cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AccountDto> GetAccountByIdQuery(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _ntegrationServicesIAccountsService.GetAccountByIdAsync(id, cancellationToken);
            return result;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AccountDto>> GetAccountsQuery(CancellationToken cancellationToken = default)
        {
            var result = await _ntegrationServicesIAccountsService.GetAccountsAsync(cancellationToken);
            return result;
        }

        public void Dispose()
        {
        }
    }
}