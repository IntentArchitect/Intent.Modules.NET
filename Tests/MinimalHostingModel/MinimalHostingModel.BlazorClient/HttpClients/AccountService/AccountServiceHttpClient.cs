using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.WebUtilities;
using MinimalHostingModel.BlazorClient.HttpClients.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.AccountController.AccountServiceHttpClientTemplate", Version = "1.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.AccountService
{
    public class AccountServiceHttpClient : IAccountService
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly HttpClient _httpClient;

        public AccountServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task Register(RegisterDto command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/Account/Register";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            request.Content = new StringContent(content, Encoding.Default, "application/json");

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<TokenResultDto> Login(LoginDto command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/Account/Login";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            request.Content = new StringContent(content, Encoding.Default, "application/json");

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await JsonSerializer.DeserializeAsync<TokenResultDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<TokenResultDto> RefreshToken(
            string authenticationToken,
            string refreshToken,
            CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/Account/RefreshToken";
            var queryParams = new Dictionary<string, string>();
            queryParams.Add("authenticationToken", authenticationToken);
            queryParams.Add("refreshToken", refreshToken);
            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);
                }
                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                {
                    return default;
                }

                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                {
                    return await JsonSerializer.DeserializeAsync<TokenResultDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task ConfirmEmail(ConfirmEmailDto command, CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/Account/ConfirmEmail";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = JsonSerializer.Serialize(command, _serializerOptions);
            request.Content = new StringContent(content, Encoding.Default, "application/json");

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task Logout(CancellationToken cancellationToken = default)
        {
            var relativeUri = $"api/Account/Logout";
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, request, response, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public void Dispose()
        {
        }
    }

    public class TokenResultDto
    {
        public string? AuthenticationToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class RegisterDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class ConfirmEmailDto
    {
        public string? UserId { get; set; }
        public string? Code { get; set; }
    }
}