using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.RequestResponse.CQRS.QueryNoInputDtoReturnCollection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryNoInputDtoReturnCollectionHandler : IRequestHandler<QueryNoInputDtoReturnCollection, List<QueryResponseDto>>
    {
        [IntentManaged(Mode.Merge)]
        public QueryNoInputDtoReturnCollectionHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<QueryResponseDto>> Handle(
            QueryNoInputDtoReturnCollection request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (QueryNoInputDtoReturnCollectionHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}