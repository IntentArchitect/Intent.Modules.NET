using System;
using System.Collections.Generic;
using System.Net;

namespace IntegrationHttpClientTestSuite.IntentGenerated.Contracts;

public class ApiException : Exception
{
    public ApiException(
        Uri requestUri, 
        HttpStatusCode statusCode, 
        IReadOnlyDictionary<string, IEnumerable<string>> responseHeaders,
        string reasonPhrase,
        string responseContent)
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
    public string ReasonPhrase { get; }
    public string ResponseContent { get; private set; }
    
    public override string ToString()
    {
        return $"Request to {RequestUri} failed with status code {(int)StatusCode} {ReasonPhrase}.{Environment.NewLine}{ResponseContent}";
    }
}