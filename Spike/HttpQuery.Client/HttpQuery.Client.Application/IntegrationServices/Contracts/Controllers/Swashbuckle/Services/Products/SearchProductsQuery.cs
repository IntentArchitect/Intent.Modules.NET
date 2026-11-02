using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace HttpQuery.Client.Application.IntegrationServices.Contracts.Controllers.Swashbuckle.Services.Products
{
    public class SearchProductsQuery
    {
        public string? NameContains { get; set; }
        public List<string>? Categories { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public static SearchProductsQuery Create(
            string? nameContains,
            List<string>? categories,
            decimal? minPrice,
            decimal? maxPrice)
        {
            return new SearchProductsQuery
            {
                NameContains = nameContains,
                Categories = categories,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
        }
    }
}