using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.Proxy.PTH.Application.Common.Exceptions;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Infrastructure.HttpClients
{
    public class DeleteClientsServiceHttpClient : IDeleteClientsService
    {
        private const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;

        public DeleteClientsServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DeleteClientAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/clients/{id}";
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