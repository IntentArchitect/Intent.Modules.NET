using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.HttpClient", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.ManyToMany
{
    public class ManyToManyHttpClient : IManyToManyService
    {
        private const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public ManyToManyHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task CreateProductItemAsync(
            CreateProductItemCommand command,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/many-to-many";
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            httpRequest.Content = new StringContent(content, Encoding.UTF8, JSON_MEDIA_TYPE);

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task UpdateProductItemAsync(
            Guid id,
            UpdateProductItemCommand command,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/many-to-many/{id}";
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            httpRequest.Content = new StringContent(content, Encoding.UTF8, JSON_MEDIA_TYPE);

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