using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor.ExampleApp.Client.HttpClients.Common;
using MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Collections;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClient", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Implementations
{
    public class CollectionsServiceHttpClient : ICollectionsService
    {
        private const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public CollectionsServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<ResponseDto>> GetDataWithCollectionParamsAsync(
            List<int> intCollection,
            List<string> stringCollection,
            int intValue,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/collections/data-with-params";

            var queryParams = new Dictionary<string, string?>();
            var intCollectionIndex = 0;

            foreach (var element in intCollection)
            {
                queryParams.Add($"intCollection[{intCollectionIndex++}]", element.ToString());
            }
            var stringCollectionIndex = 0;

            foreach (var element in stringCollection)
            {
                queryParams.Add($"stringCollection[{stringCollectionIndex++}]", element.ToString());
            }
            queryParams.Add("intValue", intValue.ToString());
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
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
                    return (await JsonSerializer.DeserializeAsync<List<ResponseDto>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public async Task<List<ResponseDto>> GetDataSingleCollectionAsync(
            List<int> intCollection,
            string stringValue,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/collections/data-single";

            var queryParams = new Dictionary<string, string?>();
            var index = 0;

            foreach (var element in intCollection)
            {
                queryParams.Add($"intCollection[{index++}]", element.ToString());
            }
            queryParams.Add("stringValue", stringValue);
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
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
                    return (await JsonSerializer.DeserializeAsync<List<ResponseDto>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
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