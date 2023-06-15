using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;
using Standard.AspNetCore.TestApplication.Application.Common.Exceptions;
using Standard.AspNetCore.TestApplication.Application.IntegrationServices.InvoiceServiceProxy;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Infrastructure.HttpClients
{
    public class InvoiceServiceProxyHttpClient : IInvoiceServiceProxyClient
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public InvoiceServiceProxyHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<Guid> CreateInvoiceAsync(InvoiceCreateDto dto, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/invoices";
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
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var str = await new StreamReader(contentStream).ReadToEndAsync().ConfigureAwait(false);
                    if (str != null && (str.StartsWith(@"""") || str.StartsWith("'"))) { str = str.Substring(1, str.Length - 2); };
                    return Guid.Parse(str);
                }
            }
        }

        public async Task<InvoiceDto> FindInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/invoices/{id}";
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
                    return await JsonSerializer.DeserializeAsync<InvoiceDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<List<InvoiceDto>> FindInvoicesAsync(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/invoices";
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
                    return await JsonSerializer.DeserializeAsync<List<InvoiceDto>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task UpdateInvoiceAsync(Guid id, InvoiceUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/invoices/{id}";
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

        public async Task DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/invoices/{id}";
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

        public void Dispose()
        {
        }
    }
}