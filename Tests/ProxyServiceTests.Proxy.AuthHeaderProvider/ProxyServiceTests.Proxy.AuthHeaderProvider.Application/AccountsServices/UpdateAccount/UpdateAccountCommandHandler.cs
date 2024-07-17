using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.UpdateAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand>
    {
        private readonly IAccountsService _accountsService;

        [IntentManaged(Mode.Merge)]
        public UpdateAccountCommandHandler(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            await _accountsService.UpdateAccountAsync(request.Id, new IntegrationServices.Contracts.OriginalServices.Services.Accounts.UpdateAccountCommand
            {
                Number = request.Number,
                Amount = new IntegrationServices.Contracts.OriginalServices.Services.Accounts.UpdateAccountMoneyDto
                {
                    Amount = request.Amount.Amount,
                    Currency = request.Amount.Currency
                },
                ClientId = request.ClientId,
                Id = request.UpdateAccountCommandId
            }, cancellationToken);
        }
    }
}