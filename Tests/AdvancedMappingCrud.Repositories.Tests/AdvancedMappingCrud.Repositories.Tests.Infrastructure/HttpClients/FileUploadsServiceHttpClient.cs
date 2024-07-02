using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices.Contracts.Services.Common;
using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices.Contracts.Services.FileUploads;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.HttpClients
{
    public class FileUploadsServiceHttpClient : IFileUploadsService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public FileUploadsServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<Guid> UploadFileAsync(
            string? contentType,
            long? contentLength,
            UploadFileCommand command,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/file-uploads/upload-file";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (contentLength != null)
            {
                httpRequest.Headers.Add("Content-Length", contentLength.ToString());
            }
            httpRequest.Content = new StreamContent(command.Content);
            httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(command.ContentType ?? "application/octet-stream");

            if (command.Filename != null)
            {
                httpRequest.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = command.Filename };
            }

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var wrappedObj = (await JsonSerializer.DeserializeAsync<JsonResponse<Guid>>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                    return wrappedObj!.Value;
                }
            }
        }

        public async Task<FileDownloadDto> DownloadFileAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/file-upload/{id}";
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