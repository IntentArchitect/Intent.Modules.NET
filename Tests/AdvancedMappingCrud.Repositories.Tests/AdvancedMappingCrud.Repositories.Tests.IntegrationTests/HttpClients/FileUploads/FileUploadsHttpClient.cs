using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.FileUploads;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.HttpClient", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.FileUploads
{
    public class FileUploadsHttpClient : IFileUploadsService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public FileUploadsHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task<Guid> UploadFileAsync(
            string? filename,
            string? contentType,
            UploadFile command,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/file-uploads/upload-file";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("filename", filename);
            queryParams.Add("contentType", contentType);
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequest.Content = new StreamContent(command.Content);

            if (command.Filename != null)
            {
                httpRequest.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(command.Filename);
            }

            if (command.ContentType != null)
            {
                httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(command.ContentType);
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

        public void Dispose()
        {
        }
    }
}