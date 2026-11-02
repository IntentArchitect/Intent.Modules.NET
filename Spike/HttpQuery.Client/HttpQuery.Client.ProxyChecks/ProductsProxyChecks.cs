using HttpQuery.Client.Application.IntegrationServices.Contracts.Controllers.Swashbuckle.Services.Products;
using HttpQuery.Client.Application.IntegrationServices.Products;
using HttpQuery.Client.Infrastructure;
using HttpQuery.Client.Infrastructure.HttpClients.Products;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HttpQuery.Client.ProxyChecks
{
    /// <summary>
    /// Calls the running HttpQuery.Controllers.Swashbuckle API through the Intent-generated proxy, wired exactly as the client
    /// app wires it (AddInfrastructure), and checks the QUERY calls go out as QUERY and return the right products.
    /// Needs HTTPQUERY_API_URL (Spike/scripts/client-check.sh sets it).
    /// </summary>
    public class ProductsProxyChecks
    {
        private readonly RecordingHandler _recorder = new();
        private readonly IProductsProxy _proxy;

        public ProductsProxyChecks()
        {
            var apiUrl = Environment.GetEnvironmentVariable("HTTPQUERY_API_URL")
                ?? throw new InvalidOperationException("Set HTTPQUERY_API_URL to the running HttpQuery.Controllers.Swashbuckle API.");
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["HttpClients:ProductsProxy:Uri"] = apiUrl })
                .Build();

            var services = new ServiceCollection();
            services.AddInfrastructure(configuration);
            services.AddHttpClient<IProductsProxy, ProductsProxyHttpClient>().AddHttpMessageHandler(() => _recorder);
            _proxy = services.BuildServiceProvider().GetRequiredService<IProductsProxy>();
        }

        [Fact]
        public async Task SearchProducts_SendsQueryAndReturnsMatches()
        {
            var ct = TestContext.Current.CancellationToken;
            var token = Guid.NewGuid().ToString("N")[..10];
            await _proxy.CreateProductAsync(CreateProductCommand.Create($"{token} bolt", "Fasteners", 10m), ct);
            await _proxy.CreateProductAsync(CreateProductCommand.Create($"{token} nut", "Fasteners", 5m), ct);
            await _proxy.CreateProductAsync(CreateProductCommand.Create($"{token} drill", "Tools", 99m), ct);

            var products = await _proxy.SearchProductsAsync(SearchProductsQuery.Create(token, ["Fasteners"], null, 20m), ct);

            Assert.Equal([$"{token} bolt", $"{token} nut"], products.Select(p => p.Name).Order());
            Assert.Contains("QUERY /api/products/search", _recorder.Requests);
        }

        [Fact]
        public async Task SearchCategoryProducts_SendsQueryWithRouteAndBody()
        {
            var ct = TestContext.Current.CancellationToken;
            var token = Guid.NewGuid().ToString("N")[..10];
            var category = $"cat-{token}";
            await _proxy.CreateProductAsync(CreateProductCommand.Create($"{token} hammer", category, 25m), ct);
            await _proxy.CreateProductAsync(CreateProductCommand.Create($"{token} saw", category, 40m), ct);

            var products = await _proxy.SearchCategoryProductsAsync(category, SearchCategoryProductsQuery.Create(category, token, 30m), ct);

            Assert.Equal([$"{token} hammer"], products.Select(p => p.Name));
            Assert.Contains($"QUERY /api/categories/{category}/products/search", _recorder.Requests);
        }

        private sealed class RecordingHandler : DelegatingHandler
        {
            public List<string> Requests { get; } = [];

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add($"{request.Method} {request.RequestUri!.AbsolutePath}");
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
