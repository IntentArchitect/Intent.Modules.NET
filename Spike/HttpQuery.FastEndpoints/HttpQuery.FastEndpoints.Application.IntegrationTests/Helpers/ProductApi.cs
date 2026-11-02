using System.Net.Http.Json;
using System.Text.Json;
using HttpQuery.FastEndpoints.Application.Products;

namespace HttpQuery.FastEndpoints.Application.IntegrationTests.Helpers
{
    /// <summary>
    /// Raw HTTP helpers for the Products endpoints. They return the response instead of throwing, so tests can assert on status codes.
    /// </summary>
    public static class ProductApi
    {
        public static readonly JsonSerializerOptions Json = JsonSerializerOptions.Web;

        public static string NewToken() => Guid.NewGuid().ToString("N")[..10];

        public static async Task<Guid> CreateProductAsync(this HttpClient client, string name, string category, decimal price, CancellationToken cancellationToken)
        {
            var response = await client.PostAsJsonAsync("api/products", new { name, category, price }, Json, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Guid>(Json, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP QUERY request with a JSON body.
        /// </summary>
        public static Task<HttpResponseMessage> QueryAsync(this HttpClient client, string uri, object body, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Query, uri)
            {
                Content = JsonContent.Create(body, options: Json)
            };
            return client.SendAsync(request, cancellationToken);
        }

        public static async Task<List<ProductDto>> ReadProductsAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return (await response.Content.ReadFromJsonAsync<List<ProductDto>>(Json, cancellationToken))!;
        }
    }
}
