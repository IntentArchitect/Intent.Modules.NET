using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Collections.GetDataSingleCollection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDataSingleCollectionQueryHandler : IRequestHandler<GetDataSingleCollectionQuery, List<ResponseDto>>
    {
        [IntentManaged(Mode.Merge)]
        public GetDataSingleCollectionQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<ResponseDto>> Handle(
            GetDataSingleCollectionQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetDataSingleCollectionQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}