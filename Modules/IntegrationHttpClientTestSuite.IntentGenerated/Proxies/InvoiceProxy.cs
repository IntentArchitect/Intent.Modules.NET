using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IntegrationHttpClientTestSuite.IntentGenerated.ClientContracts;
using IntegrationHttpClientTestSuite.IntentGenerated.ClientContracts.Invoices;
using IntegrationHttpClientTestSuite.IntentGenerated.Contracts;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClient.ServiceProxyClient", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.Proxies
{
    public class InvoiceProxy : IInvoiceService
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public InvoiceProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Create(InvoiceCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(dto, _serializerOptions);
            request.Content = new StringContent(content, Encoding.Default, "application/json");


            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<InvoiceDTO> FindById(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/{id}";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await JsonSerializer.DeserializeAsync<InvoiceDTO>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<List<InvoiceDTO>> FindAll(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await JsonSerializer.DeserializeAsync<List<InvoiceDTO>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task Update(Guid id, InvoiceUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/{id}";
            var request = new HttpRequestMessage(HttpMethod.Put, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(dto, _serializerOptions);
            request.Content = new StringContent(content, Encoding.Default, "application/json");


            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/{id}";
            var request = new HttpRequestMessage(HttpMethod.Delete, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<InvoiceDTO> QueryParamOp(string param1, int param2, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/QueryParamOp";

            var queryParams = new Dictionary<string, string>();
            queryParams.Add("param1", param1);
            queryParams.Add("param2", param2.ToString());
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);

            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await JsonSerializer.DeserializeAsync<InvoiceDTO>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task HeaderParamOp(string param1, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/HeaderParamOp";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("MY-HEADER", param1);


            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task FormParamOp(string param1, int param2, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/FormParamOp";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var formVariables = new List<KeyValuePair<string, string>>();
            formVariables.Add(new KeyValuePair<string, string>("Param1", param1));
            formVariables.Add(new KeyValuePair<string, string>("Param2", param2.ToString()));
            var content = new FormUrlEncodedContent(formVariables);
            request.Content = content;

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task RouteParamOp(string param1, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/RouteParamOp/{param1}";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task BodyParamOp(InvoiceDTO param1, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/BodyParamOp";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(param1, _serializerOptions);
            request.Content = new StringContent(content, Encoding.Default, "application/json");


            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public void Dispose()
        {
        }

        private async Task<HttpRequestException> GetHttpRequestException(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var fullRequestUri = new Uri(_httpClient.BaseAddress, request.RequestUri);
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return new HttpRequestException(
                $"Request to {fullRequestUri} failed with status code {(int)response.StatusCode} {response.ReasonPhrase}.{Environment.NewLine}{content}",
                null,
                response.StatusCode);
        }
    }
}