using System.Net;
using HttpQuery.Controllers.Swashbuckle.Application.IntegrationTests.Helpers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace HttpQuery.Controllers.Swashbuckle.Application.IntegrationTests.Products
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

            // The generated controller rejects a body whose Category differs from the route, so the body repeats it.
            var response = await client.QueryAsync($"api/categories/{category}/products/search", new { category, nameContains = token, maxPrice = 30m }, ct);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var products = await response.ReadProductsAsync(ct);
            Assert.Equal([$"{token} hammer"], products.Select(p => p.Name));
        }
    }
}
