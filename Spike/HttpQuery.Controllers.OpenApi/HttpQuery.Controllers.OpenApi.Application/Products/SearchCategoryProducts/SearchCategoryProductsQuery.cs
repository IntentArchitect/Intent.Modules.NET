using HttpQuery.Controllers.OpenApi.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace HttpQuery.Controllers.OpenApi.Application.Products.SearchCategoryProducts
{
    /// <summary>
    /// HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Category comes from the route; the remaining criteria from the request body.
    /// </summary>
    public class SearchCategoryProductsQuery : IRequest<List<ProductDto>>, IQuery
    {
        public SearchCategoryProductsQuery(string category, string? nameContains, decimal? maxPrice)
        {
            Category = category;
            NameContains = nameContains;
            MaxPrice = maxPrice;
        }

        public string Category { get; set; }
        public string? NameContains { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}