using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientRequestException", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Common.Exceptions
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

            if (responseHeaders?.TryGetValue("Content-Type", out var contentTypeValues) == true)
            {
                var contentType = contentTypeValues?.FirstOrDefault();

                if (!string.IsNullOrEmpty(contentType) && contentType.StartsWith("application/problem+json", StringComparison.OrdinalIgnoreCase))
                {
                    ProblemDetails = JsonSerializer.Deserialize<ProblemDetailsWithErrors>(responseContent);
                }
            }
        }

        public ProblemDetailsWithErrors? ProblemDetails { get; private set; }
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
            var contentHeaders = response.Content.Headers.ToDictionary(k => k.Key, v => v.Value);
            var allHeaders = headers
                .Concat(contentHeaders)
                .GroupBy(kvp => kvp.Key)
                .ToDictionary(group => group.Key, group => group.Last().Value);

            return new HttpClientRequestException(fullRequestUri, response.StatusCode, allHeaders, response.ReasonPhrase, content);
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
                message += " See content for more detail.";
            }

            return message;
        }
    }
}