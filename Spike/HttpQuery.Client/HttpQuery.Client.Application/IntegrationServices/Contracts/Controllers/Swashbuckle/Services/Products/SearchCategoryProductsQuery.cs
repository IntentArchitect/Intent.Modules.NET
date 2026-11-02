using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace HttpQuery.Client.Application.IntegrationServices.Contracts.Controllers.Swashbuckle.Services.Products
{
    public class SearchCategoryProductsQuery
    {
        public SearchCategoryProductsQuery()
        {
            Category = null!;
        }

        public string Category { get; set; }
        public string? NameContains { get; set; }
        public decimal? MaxPrice { get; set; }

        public static SearchCategoryProductsQuery Create(string category, string? nameContains, decimal? maxPrice)
        {
            return new SearchCategoryProductsQuery
            {
                Category = category,
                NameContains = nameContains,
                MaxPrice = maxPrice
            };
        }
    }
}