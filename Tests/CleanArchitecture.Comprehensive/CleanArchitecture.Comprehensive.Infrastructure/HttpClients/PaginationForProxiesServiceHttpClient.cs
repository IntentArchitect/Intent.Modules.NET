using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Exceptions;
using CleanArchitecture.Comprehensive.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.Application.IntegrationServices.Contracts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.HttpClients
{
    public class PaginationForProxiesServiceHttpClient : IPaginationForProxiesService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public PaginationForProxiesServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<PagedResult<string>> PaginatedResultAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/pagination-for-proxies/paginated-result";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("pageNo", pageNo.ToString());
            queryParams.Add("pageSize", pageSize.ToString());
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return (await JsonSerializer.DeserializeAsync<PagedResult<string>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}