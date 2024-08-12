using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Accounts.GetAccounts
{
    public class GetAccountsQuery : IRequest<List<AccountDto>>, IQuery
    {
        public GetAccountsQuery()
        {
        }
    }
}