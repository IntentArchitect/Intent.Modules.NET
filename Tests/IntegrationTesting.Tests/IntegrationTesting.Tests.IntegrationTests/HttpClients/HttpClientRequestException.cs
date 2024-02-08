using System.Net;
using System.Text.Json;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.HttpClientRequestException", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients
{
    public class HttpClientRequestException : Exception
    {
        public HttpClientRequestException(Uri requestUri,
            HttpStatusCode statusCode,
            IReadOnlyDictionary<string, IEnumerable<string>> responseHeaders,
            string? reasonPhrase,
            string responseContent) : base(GetMessage(requestUri, statusCode, reasonPhrase, responseContent))
        {
            RequestUri = requestUri;
            StatusCode = statusCode;
            ResponseHeaders = responseHeaders;
            ReasonPhrase = reasonPhrase;
            ResponseContent = responseContent;
        }

        public Uri RequestUri { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public IReadOnlyDictionary<string, IEnumerable<string>> ResponseHeaders { get; private set; }
        public string? ReasonPhrase { get; private set; }
        public string ResponseContent { get; private set; }

        public static async Task<HttpClientRequestException> Create(
            Uri baseAddress,
            HttpRequestMessage request,
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            var fullRequestUri = new Uri(baseAddress, request.RequestUri!);
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var headers = response.Headers.ToDictionary(k => k.Key, v => v.Value);
            return new HttpClientRequestException(fullRequestUri, response.StatusCode, headers, response.ReasonPhrase, content);
        }

        private static string GetMessage(
            Uri requestUri,
            HttpStatusCode statusCode,
            string? reasonPhrase,
            string responseContent)
        {
            var message = $"Request to {requestUri} failed with status code {(int)statusCode} {reasonPhrase}.";
            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                message = FormatJson(JsonSerializer.Deserialize<dynamic>(responseContent))
                    .Replace("\\r\\n", Environment.NewLine);
            }
            return message;
        }

        private static string FormatJson(object jsonObject)
        {
            return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions() { WriteIndented = true });
        }
    }
}