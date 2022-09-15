using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using IntegrationHttpClientTestSuite.IntentGenerated.ClientContracts.InvoiceProxy;
using IntegrationHttpClientTestSuite.IntentGenerated.Exceptions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.HttpClients
{
    public class InvoiceProxyHttpClient : IInvoiceProxyClient
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public InvoiceProxyHttpClient(HttpClient httpClient)
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
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
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        public async Task ThrowsException(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/ThrowsException";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        public async Task<Guid> GetWrappedPrimitiveGuid(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/GetWrappedPrimitiveGuid";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var wrappedObj = await JsonSerializer.DeserializeAsync<JsonResponse<Guid>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                    return wrappedObj.Value;
                }
            }
        }
        public async Task<string> GetWrappedPrimitiveString(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/GetWrappedPrimitiveString";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var wrappedObj = await JsonSerializer.DeserializeAsync<JsonResponse<string>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                    return wrappedObj.Value;
                }
            }
        }
        public async Task<int> GetWrappedPrimitiveInt(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/GetWrappedPrimitiveInt";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var wrappedObj = await JsonSerializer.DeserializeAsync<JsonResponse<int>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                    return wrappedObj.Value;
                }
            }
        }
        public async Task<Guid> GetPrimitiveGuid(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/GetPrimitiveGuid";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);
                    if (str.StartsWith(@"""") || str.StartsWith("'")) { str = str.Substring(1, str.Length - 2); }
                    return Guid.Parse(str);
                }
            }
        }
        public async Task<string> GetPrimitiveString(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/GetPrimitiveString";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);
                    if (str.StartsWith(@"""") || str.StartsWith("'")) { str = str.Substring(1, str.Length - 2); }
                    return str;
                }
            }
        }
        public async Task<int> GetPrimitiveInt(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"/api/Invoice/GetPrimitiveInt";
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);
                    if (str.StartsWith(@"""") || str.StartsWith("'")) { str = str.Substring(1, str.Length - 2); }
                    return int.Parse(str);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}