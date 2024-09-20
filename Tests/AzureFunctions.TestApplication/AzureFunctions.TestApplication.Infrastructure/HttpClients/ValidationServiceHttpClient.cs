using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Exceptions;
using AzureFunctions.TestApplication.Application.IntegrationServices;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace AzureFunctions.TestApplication.Infrastructure.HttpClients
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
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task InboundComValAsync(InboundComValCommand command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"validation/inbound";
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

        public void Dispose()
        {
        }
    }
}