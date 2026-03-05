using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AspNetControllers.SecuredByDefault.Application.Common.Exceptions;
using AspNetControllers.SecuredByDefault.Application.IntegrationServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace AspNetControllers.SecuredByDefault.Infrastructure.HttpClients
{
    public class TestServiceHttpClient : ITestService
    {
        private const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;

        public TestServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task OperationAsync(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/test/operation";
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
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