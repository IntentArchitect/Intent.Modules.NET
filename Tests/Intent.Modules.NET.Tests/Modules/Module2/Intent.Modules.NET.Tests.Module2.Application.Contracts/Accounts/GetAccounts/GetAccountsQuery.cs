using Intent.Modules.NET.Tests.Application.Core.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.GetAccounts
{
    public class GetAccountsQuery : IRequest<List<AccountDto>>, IQuery
    {
        public GetAccountsQuery()
        {
        }
    }
}