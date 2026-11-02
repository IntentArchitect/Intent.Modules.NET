using HttpQuery.Controllers.Swashbuckle.Domain.Repositories.Products;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace HttpQuery.Controllers.Swashbuckle.Application.Products.SearchProducts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public SearchProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Filters products by the criteria in the request body; every criterion is optional.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var categories = request.Categories is { Count: > 0 } ? request.Categories : null;

            return await _productRepository.FindAllProjectToAsync<ProductDto>(
                p => (request.NameContains == null || p.Name.Contains(request.NameContains))
                && (categories == null || categories.Contains(p.Category))
                && (request.MinPrice == null || p.Price >= request.MinPrice)
                && (request.MaxPrice == null || p.Price <= request.MaxPrice),
                q => q.OrderBy(p => p.Name),
                cancellationToken);
        }
    }
}
