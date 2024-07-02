using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Common;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClient", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Implementations
{
    public class NamedQueryStringsServiceHttpClient : INamedQueryStringsService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public NamedQueryStringsServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task NamedQueryStringsAsync(string par1, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/named-query-strings/named-query-strings";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("custom-querystring-name", par1);
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
        }
    }
}