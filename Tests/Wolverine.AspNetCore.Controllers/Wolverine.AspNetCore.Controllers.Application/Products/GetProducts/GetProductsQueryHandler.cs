using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.Products.GetProducts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetProductsQueryHandler
    {
        [IntentManaged(Mode.Merge)]
        public GetProductsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetProductsQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}