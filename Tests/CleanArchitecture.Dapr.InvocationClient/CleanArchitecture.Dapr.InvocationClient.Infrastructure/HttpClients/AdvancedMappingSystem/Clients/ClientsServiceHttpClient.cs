using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CleanArchitecture.Dapr.InvocationClient.Application.Common.Exceptions;
using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices;
using CleanArchitecture.Dapr.InvocationClient.Application.IntegrationServices.Contracts.Services.AdvancedMappingSystem.Clients;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.ServiceInvocation.HttpClient", Version = "2.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Infrastructure.HttpClients.AdvancedMappingSystem.Clients
{
    public class ClientsServiceHttpClient : IClientsService
    {
        private const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public ClientsServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                { new JsonStringEnumConverter() }
            };
        }

        public async Task<string> CreateClientAsync(
            CreateClientCommand command,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/advanced-mapping-system/client";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            httpRequest.Content = new StringContent(content, Encoding.UTF8, JSON_MEDIA_TYPE);

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var wrappedObj = (await JsonSerializer.DeserializeAsync<JsonResponse<string>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                    return wrappedObj!.Value;
                }
            }
        }

        public async Task DeleteClientAsync(DeleteClientCommand command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/advanced-mapping-system/client/{command.Id}";
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<ClientDto> GetClientByIdAsync(
            GetClientByIdQuery query,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/advanced-mapping-system/client/{query.Id}";
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return (await JsonSerializer.DeserializeAsync<ClientDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public async Task<List<ClientDto>> GetClientsAsync(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/advanced-mapping-system/client";
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return (await JsonSerializer.DeserializeAsync<List<ClientDto>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public async Task UpdateClientAsync(UpdateClientCommand command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/advanced-mapping-system/client/{command.Id}";
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, relativeUri);
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