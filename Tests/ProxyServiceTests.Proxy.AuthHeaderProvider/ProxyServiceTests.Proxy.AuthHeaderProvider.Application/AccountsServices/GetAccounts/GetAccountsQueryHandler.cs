using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.GetAccounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
    {
        private readonly IAccountsService _accountsService;

        [IntentManaged(Mode.Merge)]
        public GetAccountsQueryHandler(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            var result = await _accountsService.GetAccountsAsync(cancellationToken);
            return result;
        }
    }
}