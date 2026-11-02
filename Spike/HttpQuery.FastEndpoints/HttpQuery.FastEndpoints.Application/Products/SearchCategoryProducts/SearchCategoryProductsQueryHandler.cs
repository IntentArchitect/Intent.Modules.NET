using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Application.Products.SearchCategoryProducts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchCategoryProductsQueryHandler : IRequestHandler<SearchCategoryProductsQuery, List<ProductDto>>
    {
        [IntentManaged(Mode.Merge)]
        public SearchCategoryProductsQueryHandler()
        {
        }

        /// <summary>
        /// HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Category comes from the route; the remaining criteria from the request body.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ProductDto>> Handle(
            SearchCategoryProductsQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (SearchCategoryProductsQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}