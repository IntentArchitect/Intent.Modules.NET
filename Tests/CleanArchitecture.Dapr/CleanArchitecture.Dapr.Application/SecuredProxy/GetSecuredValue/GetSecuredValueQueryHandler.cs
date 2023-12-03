using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.SecuredProxy.GetSecuredValue
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetSecuredValueQueryHandler : IRequestHandler<GetSecuredValueQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public GetSecuredValueQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(GetSecuredValueQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}