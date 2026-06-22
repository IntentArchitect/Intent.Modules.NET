using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.Products.GetProductById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductByIdQueryHandler
    {
        [IntentManaged(Mode.Merge)]
        public GetProductByIdQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<ProductDto> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetProductByIdQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}