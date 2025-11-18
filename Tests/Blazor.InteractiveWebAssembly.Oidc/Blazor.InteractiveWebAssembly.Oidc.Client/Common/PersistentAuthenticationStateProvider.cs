using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazor.InteractiveWebAssembly.Oidc.Client.Components.Account;
using Blazor.InteractiveWebAssembly.Oidc.Client.Components.Account.Shared;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Client.PersistentAuthenticationStateProviderTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Oidc.Client.Common
{
    public class PersistentAuthenticationStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        private static readonly Task<AuthenticationState> _defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        private readonly Task<AuthenticationState> _authenticationStateTask = _defaultUnauthenticatedTask;
        private readonly Uri? _identityUrl;
        private readonly HttpClient _refreshClient = new HttpClient();
        private readonly NavigationManager _nav;
        private string? _accessToken;
        private string? _refreshToken;
        private DateTimeOffset _accessTokenExpiresAt = DateTimeOffset.MinValue;

        public PersistentAuthenticationStateProvider(PersistentComponentState state, NavigationManager nav)
        {
            _nav = nav;
            if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
            {
                return;
            }
            Claim[] claims = [
                                        new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                                        new Claim(ClaimTypes.Email, userInfo.Email),
                                        new Claim("access_token", userInfo.AccessToken == null ? "" : userInfo.AccessToken) ];
            _authenticationStateTask = Task.FromResult(
                                        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                                            authenticationType: nameof(PersistentAuthenticationStateProvider)))));

            if (!string.IsNullOrWhiteSpace(userInfo.AccessToken))
            {
                _accessToken = userInfo.AccessToken;
                _refreshToken = userInfo.RefreshToken;

                if (userInfo.AccessTokenExpiresAt.HasValue)
                {
                    _accessTokenExpiresAt = userInfo.AccessTokenExpiresAt.Value;
                }

                if (!string.IsNullOrEmpty(userInfo.RefreshUrl))
                {
                    _identityUrl = new Uri(userInfo.RefreshUrl, UriKind.Absolute);
                }
            }
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return _authenticationStateTask;
        }

        public ValueTask<AccessTokenResult> RequestAccessToken()
=> RequestAccessToken(new AccessTokenRequestOptions());

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            var missingToken = string.IsNullOrWhiteSpace(_accessToken);
            var expired = _accessTokenExpiresAt > DateTimeOffset.MinValue && _accessTokenExpiresAt <= DateTimeOffset.UtcNow;

            if (missingToken || expired)
            {
                // Try to refresh if we have a refresh token
                if (!string.IsNullOrWhiteSpace(_refreshToken))
                {
                    var refreshed = await TryRefreshAccessTokenAsync();
                    if (refreshed)
                    {
                        // we now have a new _accessToken / _accessTokenExpiresAt
                        var at = new AccessToken
                        {
                            Value = _accessToken!,
                            Expires = _accessTokenExpiresAt
                        };

                        return new AccessTokenResult(AccessTokenResultStatus.Success, at, null, null);
                    }
                }

                // No refresh token OR refresh failed → send user to login
                var current = _nav.ToBaseRelativePath(_nav.Uri);
                var returnUrl = "/" + current;
                var loginUrl = $"/account/login?returnUrl={Uri.EscapeDataString(returnUrl)}";

                _nav.NavigateTo(loginUrl, forceLoad: true);

                return new AccessTokenResult(
                    AccessTokenResultStatus.RequiresRedirect, null, loginUrl, null);
            }

            // Token present and we consider it valid
            var expires = _accessTokenExpiresAt > DateTimeOffset.MinValue
                            ? _accessTokenExpiresAt
                            : DateTimeOffset.UtcNow.AddMinutes(5);

            var accessToken = new AccessToken
            {
                Value = _accessToken!,
                Expires = expires
            };
            return new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null);
        }

        private async Task<bool> TryRefreshAccessTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(_refreshToken) || _identityUrl == null)
                return false;

            try
            {
                var refreshUri = new Uri(_identityUrl, "refresh"); // e.g. https://ids.example.com/refresh

                var response = await _refreshClient.PostAsJsonAsync(refreshUri, new
                {
                    refreshToken = _refreshToken
                });

                if (!response.IsSuccessStatusCode)
                    return false;

                var dto = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
                if (dto is null || string.IsNullOrWhiteSpace(dto.AccessToken))
                    return false;

                _accessToken = dto.AccessToken;
                _refreshToken = string.IsNullOrWhiteSpace(dto.RefreshToken)
                                ? _refreshToken // keep old if not rotated
                                : dto.RefreshToken;

                // compute expiry
                _accessTokenExpiresAt = new DateTimeOffset(dto.ExpiresIn!.Value, TimeSpan.Zero);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}