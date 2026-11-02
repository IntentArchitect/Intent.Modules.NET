using System.Net;
using HttpQuery.FastEndpoints.Application.IntegrationTests.Helpers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Application.IntegrationTests.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class SearchCategoryProductsTests : BaseIntegrationTest
    {
        public SearchCategoryProductsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Query_WithRouteAndBody_ReturnsProductsInCategory()
        {
            var ct = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = ProductApi.NewToken();
            var category = $"cat-{token}";
            await client.CreateProductAsync($"{token} hammer", category, 25m, ct);
            await client.CreateProductAsync($"{token} saw", category, 40m, ct);
            await client.CreateProductAsync($"{token} glue", "Other", 3m, ct);

            // FastEndpoints binds Category from the route; the body carries the remaining criteria.
            var response = await client.QueryAsync($"api/categories/{category}/products/search", new { nameContains = token, maxPrice = 30m }, ct);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var products = await response.ReadProductsAsync(ct);
            Assert.Equal([$"{token} hammer"], products.Select(p => p.Name));
        }
    }
}
