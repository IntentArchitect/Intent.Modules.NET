using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Exceptions;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices;
using Subscribe.CleanArchDapr.TestApplication.Application.IntegrationServices.Publish.CleanArchDapr.TestApplication.Services.Orders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.ServiceInvocation.HttpClient", Version = "2.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Infrastructure.HttpClients
{
    public class MyProxyHttpClient : IMyProxy
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public MyProxyHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task OrderConfirmedAsync(Guid id, OrderConfirmed command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/orders/{id}/confirmed";
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
            }
        }

        public void Dispose()
        {
        }
    }
}