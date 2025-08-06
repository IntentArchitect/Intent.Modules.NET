using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Collections.GetDataWithCollectionParams
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDataWithCollectionParamsQueryHandler : IRequestHandler<GetDataWithCollectionParamsQuery, List<ResponseDto>>
    {
        [IntentManaged(Mode.Merge)]
        public GetDataWithCollectionParamsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<ResponseDto>> Handle(
            GetDataWithCollectionParamsQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetDataWithCollectionParamsQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}