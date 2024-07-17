using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.GetAccountById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto>
    {
        private readonly IAccountsService _accountsService;

        [IntentManaged(Mode.Merge)]
        public GetAccountByIdQueryHandler(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _accountsService.GetAccountByIdAsync(request.Id, cancellationToken);
            return result;
        }
    }
}