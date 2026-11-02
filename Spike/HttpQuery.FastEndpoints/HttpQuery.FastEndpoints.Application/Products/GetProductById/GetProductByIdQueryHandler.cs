using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Application.Products.GetProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetProductByIdQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetProductByIdQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}