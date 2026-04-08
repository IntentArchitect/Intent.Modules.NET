using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.GetAccountByCollection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAccountByCollectionQueryHandler : IRequestHandler<GetAccountByCollectionQuery, AccountDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetAccountByCollectionQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<AccountDto> Handle(GetAccountByCollectionQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetAccountByCollectionQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}