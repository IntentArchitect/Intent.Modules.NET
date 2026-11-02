using HttpQuery.Controllers.Swashbuckle.Domain.Repositories.Products;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace HttpQuery.Controllers.Swashbuckle.Application.Products.SearchCategoryProducts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchCategoryProductsQueryHandler : IRequestHandler<SearchCategoryProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        [IntentManaged(Mode.Merge)]
        public SearchCategoryProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Category comes from the route; the remaining criteria from the request body.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ProductDto>> Handle(
            SearchCategoryProductsQuery request,
            CancellationToken cancellationToken)
        {
            return await _productRepository.FindAllProjectToAsync<ProductDto>(
                p => p.Category == request.Category
                && (request.NameContains == null || p.Name.Contains(request.NameContains))
                && (request.MaxPrice == null || p.Price <= request.MaxPrice),
                q => q.OrderBy(p => p.Name),
                cancellationToken);
        }
    }
}
