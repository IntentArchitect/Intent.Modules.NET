using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.DeleteAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IAccountsService _accountsService;

        [IntentManaged(Mode.Merge)]
        public DeleteAccountCommandHandler(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            await _accountsService.DeleteAccountAsync(request.Id, cancellationToken);
        }
    }
}