using System;
using System.Collections.Generic;
using System.Net;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClient.RequestHttpException", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.Exceptions
{
    public class RequestHttpException : Exception
    {
        public RequestHttpException(
            Uri requestUri,
            HttpStatusCode statusCode,
            IReadOnlyDictionary<string, IEnumerable<string>> responseHeaders,
            string reasonPhrase,
            string responseContent)
            : base(GetMessage(requestUri, statusCode, reasonPhrase, responseContent))
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
        public string ReasonPhrase { get; private set; }
        public string ResponseContent { get; private set; }

        private static string GetMessage(Uri requestUri, HttpStatusCode statusCode, string reasonPhrase, string responseContent)
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