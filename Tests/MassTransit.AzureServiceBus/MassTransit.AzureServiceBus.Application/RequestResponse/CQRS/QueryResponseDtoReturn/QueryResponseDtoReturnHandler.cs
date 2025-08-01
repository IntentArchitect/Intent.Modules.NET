using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.QueryResponseDtoReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class QueryResponseDtoReturnHandler : IRequestHandler<QueryResponseDtoReturn, QueryResponseDto>
    {
        [IntentManaged(Mode.Merge)]
        public QueryResponseDtoReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<QueryResponseDto> Handle(QueryResponseDtoReturn request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (QueryResponseDtoReturnHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}