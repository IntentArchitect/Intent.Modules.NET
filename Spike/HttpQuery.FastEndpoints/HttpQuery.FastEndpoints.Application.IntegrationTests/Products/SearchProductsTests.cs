using System.Net;
using System.Net.Http.Json;
using HttpQuery.FastEndpoints.Application.IntegrationTests.Helpers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace HttpQuery.FastEndpoints.Application.IntegrationTests.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class SearchProductsTests : BaseIntegrationTest
    {
        public SearchProductsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Query_WithBodyCriteria_ReturnsMatchingProducts()
        {
            var ct = TestContext.Current.CancellationToken;
            var client = CreateClient();
            var token = ProductApi.NewToken();
            await client.CreateProductAsync($"{token} bolt", "Fasteners", 10m, ct);
            await client.CreateProductAsync($"{token} nut", "Fasteners", 5m, ct);
            await client.CreateProductAsync($"{token} drill", "Tools", 99m, ct);

            var response = await client.QueryAsync("api/products/search", new { nameContains = token, categories = new[] { "Fasteners" }, maxPrice = 20m }, ct);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var products = await response.ReadProductsAsync(ct);
            Assert.Equal([$"{token} bolt", $"{token} nut"], products.Select(p => p.Name).Order());
        }

        [Fact]
        public async Task Query_WithEmptyBody_ReturnsOk()
        {
            var ct = TestContext.Current.CancellationToken;
            var response = await CreateClient().QueryAsync("api/products/search", new { }, ct);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_OnQueryRoute_Returns405()
        {
            var ct = TestContext.Current.CancellationToken;
            var response = await CreateClient().PostAsJsonAsync("api/products/search", new { }, ProductApi.Json, ct);

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
    }
}
