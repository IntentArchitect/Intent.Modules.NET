using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Exceptions;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.QueryDtoParameter;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.HttpClients.QueryDtoParameter
{
    public class QueryDtoParameterServiceHttpClient : IQueryDtoParameterService
    {
        private const string JSON_MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;

        public QueryDtoParameterServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> HasDtoParameterAsync(
            QueryDtoParameterCriteria arg,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/query-dto-parameter/new";

            var queryParams = new Dictionary<string, string?>();
            ArgumentNullException.ThrowIfNull(arg);
            queryParams.Add("field1", arg.Field1);
            queryParams.Add("field2", arg.Field2);

            if (arg.Nested?.Numbers != null)
            {
                var numbersIndex = 0;

                foreach (var element in arg.Nested.Numbers)
                {
                    queryParams.Add($"nested.numbers[{numbersIndex++}]", element.ToString());
                }
            }

            if (arg.Nested?.NullableProp != null)
            {
                queryParams.Add("nested.nullableProp", arg.Nested.NullableProp);
            }
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, relativeUri);
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
                    return int.Parse(str);
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