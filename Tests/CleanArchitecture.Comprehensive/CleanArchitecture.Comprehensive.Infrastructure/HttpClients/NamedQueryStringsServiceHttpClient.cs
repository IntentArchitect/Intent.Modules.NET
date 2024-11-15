using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Exceptions;
using CleanArchitecture.Comprehensive.Application.IntegrationServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.HttpClients
{
    public class NamedQueryStringsServiceHttpClient : INamedQueryStringsService
    {
        public const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;

        public NamedQueryStringsServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task NamedQueryStringsAsync(string par1, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/named-query-strings/named-query-strings";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("custom-querystring-name", par1);
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Class cleanup goes here
        }
    }
}