using System.Net.Http.Json;
using System.Text.Json;

namespace IntegrationTesting.SQLLite.Tests.Application.IntegrationTests.Harness
{
    /// <summary>
    /// Thin helpers over <see cref="HttpClient"/> that send a request and return the
    /// <see cref="HttpResponseMessage"/> WITHOUT throwing on a non-success status code — the tests
    /// assert on 4xx responses as much as they do on 2xx ones, so the status must stay observable.
    /// </summary>
    public static class ApiClientExtensions
    {
        /// <summary>
        /// Matches the ASP.NET Core defaults the server serializes with (camelCase on the wire,
        /// case-insensitive on read). A mismatch here surfaces as a phantom behavioural failure.
        /// </summary>
        public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public static Task<HttpResponseMessage> PostAsJsonNoThrowAsync<TBody>(
            this HttpClient client,
            string requestUri,
            TBody body,
            CancellationToken cancellationToken) =>
            client.PostAsJsonAsync(requestUri, body, JsonOptions, cancellationToken);

        public static Task<HttpResponseMessage> PutAsJsonNoThrowAsync<TBody>(
            this HttpClient client,
            string requestUri,
            TBody body,
            CancellationToken cancellationToken) =>
            client.PutAsJsonAsync(requestUri, body, JsonOptions, cancellationToken);

        public static Task<HttpResponseMessage> GetNoThrowAsync(
            this HttpClient client,
            string requestUri,
            CancellationToken cancellationToken) =>
            client.GetAsync(requestUri, cancellationToken);

        public static Task<HttpResponseMessage> DeleteNoThrowAsync(
            this HttpClient client,
            string requestUri,
            CancellationToken cancellationToken) =>
            client.DeleteAsync(requestUri, cancellationToken);

        /// <summary>
        /// Reads a success body using the server's serializer settings.
        /// </summary>
        public static async Task<T> ReadContentAsync<T>(
            this HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
            Assert.NotNull(value);
            return value;
        }

        /// <summary>
        /// Reads the raw error/problem body, for negative assertions and failure diagnostics.
        /// </summary>
        public static Task<string> ReadErrorBodyAsync(
            this HttpResponseMessage response,
            CancellationToken cancellationToken) =>
            response.Content.ReadAsStringAsync(cancellationToken);
    }
}
