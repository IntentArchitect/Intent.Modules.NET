using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.GetAccountById
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