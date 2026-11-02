using HttpQuery.FastEndpoints.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Application.Products.SearchProducts
{
    /// <summary>
    /// HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Filters products by the criteria in the request body; every criterion is optional.
    /// </summary>
    public class SearchProductsQuery : IRequest<List<ProductDto>>, IQuery
    {
        public SearchProductsQuery(string? nameContains, List<string>? categories, decimal? minPrice, decimal? maxPrice)
        {
            NameContains = nameContains;
            Categories = categories;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }

        public string? NameContains { get; set; }
        public List<string>? Categories { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}