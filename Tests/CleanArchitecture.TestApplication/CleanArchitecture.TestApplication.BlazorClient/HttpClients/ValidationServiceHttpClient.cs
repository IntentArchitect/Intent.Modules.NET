using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CleanArchitecture.TestApplication.BlazorClient.HttpClients.Common;
using CleanArchitecture.TestApplication.BlazorClient.HttpClients.Services.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClient", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients
{
    public class ValidationServiceHttpClient : IValidationService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public ValidationServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<ValidatedResultDto?> ResultValidationsAsync(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/validation/result-validations";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await JsonSerializer.DeserializeAsync<ValidatedResultDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}