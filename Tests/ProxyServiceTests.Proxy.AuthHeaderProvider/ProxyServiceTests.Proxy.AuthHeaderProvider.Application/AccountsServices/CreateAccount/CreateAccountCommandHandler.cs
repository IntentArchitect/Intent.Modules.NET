using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.CreateAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountsService _accountsService;

        [IntentManaged(Mode.Merge)]
        public CreateAccountCommandHandler(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountsService.CreateAccountAsync(new IntegrationServices.Contracts.OriginalServices.Services.Accounts.CreateAccountCommand
            {
                Number = request.Number,
                Amount = new IntegrationServices.Contracts.OriginalServices.Services.Accounts.CreateAccountMoneyDto
                {
                    Amount = request.Amount.Amount,
                    Currency = request.Amount.Currency
                },
                ClientId = request.ClientId
            }, cancellationToken);
            return result;
        }
    }
}