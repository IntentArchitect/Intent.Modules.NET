using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Common.Exceptions;
using AzureFunctions.NET8.Application.IntegrationServices;
using AzureFunctions.NET8.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace AzureFunctions.NET8.Infrastructure.HttpClients
{
    public class NullableResultServiceHttpClient : INullableResultService
    {
        private const string JSON_MEDIA_TYPE = "application/json";
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public NullableResultServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<CustomerDto> GetCustomerNullableAsync(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"nullable-result/customer-nullable";
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await JsonSerializer.DeserializeAsync<CustomerDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
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