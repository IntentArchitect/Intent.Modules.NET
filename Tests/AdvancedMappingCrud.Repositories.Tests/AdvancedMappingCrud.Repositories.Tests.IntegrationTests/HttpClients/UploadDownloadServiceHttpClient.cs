using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Common;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.HttpClient", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients
{
    public class UploadDownloadServiceHttpClient : IUploadDownloadService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public UploadDownloadServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<Guid> UploadAsync(
            Stream content,
            string? filename,
            string? contentType,
            long? contentLength,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/upload-download/upload";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (contentLength != null)
            {
                httpRequest.Headers.Add("Content-Length", contentLength.ToString());
            }
            httpRequest.Content = new StreamContent(content);
            httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType ?? "application/octet-stream");

            if (filename != null)
            {
                httpRequest.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = filename };
            }

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var str = await new StreamReader(contentStream).ReadToEndAsync(cancellationToken).ConfigureAwait(false);

                    if (str.StartsWith(@"""") || str.StartsWith("'"))
                    {
                        str = str.Substring(1, str.Length - 2);
                    }
                    return Guid.Parse(str);
                }
            }
        }

        public async Task<FileDownloadDto> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/upload-download/download";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("id", id.ToString("D"));
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }
                var memoryStream = new MemoryStream();
                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                await responseStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return FileDownloadDto.Create(memoryStream, filename: response.Content.Headers.ContentDisposition?.FileName, contentType: response.Content.Headers.ContentType?.MediaType ?? "");
            }
        }

        public void Dispose()
        {
        }
    }
}