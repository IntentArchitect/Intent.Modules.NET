using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Basics;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.HttpClient", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics
{
    public class BasicsHttpClient : IBasicsService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public BasicsHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<Guid> CreateBasicAsync(CreateBasicCommand command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/basics";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var wrappedObj = (await JsonSerializer.DeserializeAsync<JsonResponse<Guid>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                    return wrappedObj!.Value;
                }
            }
        }

        public async Task DeleteBasicAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/basics/{id}";
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task UpdateBasicAsync(
            Guid id,
            UpdateBasicCommand command,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/basics/{id}";
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<BasicDto> GetBasicByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/basics/{id}";
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
                    return (await JsonSerializer.DeserializeAsync<BasicDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public async Task<PagedResult<BasicDto>> GetBasicsNullableAsync(
            int pageNo,
            int pageSize,
            string? orderBy,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/basics/nullable";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("pageNo", pageNo.ToString());
            queryParams.Add("pageSize", pageSize.ToString());
            queryParams.Add("orderBy", orderBy);
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
                    return (await JsonSerializer.DeserializeAsync<PagedResult<BasicDto>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public async Task<PagedResult<BasicDto>> GetBasicsAsync(
            int pageNo,
            int pageSize,
            string orderBy,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/basics";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("pageNo", pageNo.ToString());
            queryParams.Add("pageSize", pageSize.ToString());
            queryParams.Add("orderBy", orderBy);
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
                    return (await JsonSerializer.DeserializeAsync<PagedResult<BasicDto>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}