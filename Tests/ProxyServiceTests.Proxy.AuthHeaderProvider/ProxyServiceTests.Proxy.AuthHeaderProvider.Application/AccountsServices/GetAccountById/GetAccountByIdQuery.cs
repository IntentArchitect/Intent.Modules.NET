using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.Common.Interfaces;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.GetAccountById
{
    public class GetAccountByIdQuery : IRequest<AccountDto>, IQuery
    {
        public GetAccountByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}