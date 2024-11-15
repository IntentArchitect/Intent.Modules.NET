using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Common.Exceptions;
using CleanArchitecture.Comprehensive.Application.IntegrationServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Infrastructure.HttpClients
{
    public class ParamConversionServiceHttpClient : IParamConversionService
    {
        public const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;

        public ParamConversionServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckTypeConversionsOnProxyAsync(
            DateTime from,
            DateTime? to,
            Guid id,
            decimal value,
            TimeSpan time,
            bool active,
            DateOnly justDate,
            DateTimeOffset otherDate,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/param-conversion/check-type-conversions-on-proxy";

            var queryParams = new Dictionary<string, string?>();
            queryParams.Add("from", from.ToString("o"));
            queryParams.Add("to", to?.ToString("o"));
            queryParams.Add("id", id.ToString("D"));
            queryParams.Add("value", value.ToString(CultureInfo.InvariantCulture));
            queryParams.Add("time", time.ToString("c"));
            queryParams.Add("active", active.ToString().ToLowerInvariant());
            queryParams.Add("justDate", justDate.ToString("o"));
            queryParams.Add("otherDate", otherDate.ToString("o"));
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    var str = await new StreamReader(contentStream).ReadToEndAsync(cancellationToken).ConfigureAwait(false);

                    if (str.StartsWith('"') || str.StartsWith('\''))
                    {
                        str = str[1..^1];
                    }
                    return bool.Parse(str);
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