using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Exceptions;
using AzureFunctions.TestApplication.Application.IntegrationServices;
using AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Validation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace AzureFunctions.TestApplication.Infrastructure.HttpClients
{
    public class ValidationServiceHttpClient : IValidationService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public ValidationServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task InboundComValAsync(InboundComValCommand command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"validation/inbound";
            var httpRequest = new HttpRequestMessage(HttpMethod.Put, relativeUri);
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

        public async Task<DummyResultDto> InboundQueValAsync(
            string rangeStr,
            string minStr,
            string maxStr,
            int rangeInt,
            int minInt,
            int maxInt,
            string isRequired,
            string isRequiredEmpty,
            decimal decimalRange,
            decimal decimalMin,
            decimal decimalMax,
            string stringOption,
            string stringOptionNonEmpty,
            EnumDescriptions myEnum,
            string regexField,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"validation/inbound-validation";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("rangeStr", rangeStr);
            queryParams.Add("minStr", minStr);
            queryParams.Add("maxStr", maxStr);
            queryParams.Add("rangeInt", rangeInt.ToString());
            queryParams.Add("minInt", minInt.ToString());
            queryParams.Add("maxInt", maxInt.ToString());
            queryParams.Add("isRequired", isRequired);
            queryParams.Add("isRequiredEmpty", isRequiredEmpty);
            queryParams.Add("decimalRange", decimalRange.ToString(CultureInfo.InvariantCulture));
            queryParams.Add("decimalMin", decimalMin.ToString(CultureInfo.InvariantCulture));
            queryParams.Add("decimalMax", decimalMax.ToString(CultureInfo.InvariantCulture));
            queryParams.Add("stringOption", stringOption);
            queryParams.Add("stringOptionNonEmpty", stringOptionNonEmpty);
            queryParams.Add("myEnum", myEnum.ToString());
            queryParams.Add("regexField", regexField);
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return (await JsonSerializer.DeserializeAsync<DummyResultDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}