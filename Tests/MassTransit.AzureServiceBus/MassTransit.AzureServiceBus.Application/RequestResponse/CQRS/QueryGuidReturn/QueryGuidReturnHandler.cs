using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryGuidReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryGuidReturnHandler : IRequestHandler<QueryGuidReturn, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public QueryGuidReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(QueryGuidReturn request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}