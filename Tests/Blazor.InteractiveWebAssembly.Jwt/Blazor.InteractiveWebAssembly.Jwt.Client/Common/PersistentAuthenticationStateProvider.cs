using System.Net.Http.Json;
using System.Security.Claims;
using Blazor.InteractiveWebAssembly.Jwt.Client.Components.Account;
using Blazor.InteractiveWebAssembly.Jwt.Client.Components.Account.Shared;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Client.PersistentAuthenticationStateProviderTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Client.Common
{
    public class PersistentAuthenticationStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        private static readonly Task<AuthenticationState> _defaultUnauthenticatedTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        private Task<AuthenticationState> _authenticationStateTask = _defaultUnauthenticatedTask;
        private Uri? _identityUrl;
        private readonly HttpClient _refreshClient = new HttpClient();
        private readonly NavigationManager _nav;
        private string? _accessToken;
        private string? _refreshToken;
        private DateTimeOffset _accessTokenExpiresAt = DateTimeOffset.MinValue;
        private string? _userId;
        private string? _email;

        public PersistentAuthenticationStateProvider(PersistentComponentState state, NavigationManager nav)
        {
            _nav = nav;
            if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
            {
                return;
            }
            _userId = userInfo.UserId;
            _email = userInfo.Email;
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

                if (userInfo.AccessTokenExpiresAt.HasValue)
                {
                    _accessTokenExpiresAt = userInfo.AccessTokenExpiresAt.Value;
                }
                _refreshToken = userInfo.RefreshToken;

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

        public ValueTask<AccessTokenResult> RequestAccessToken() => RequestAccessToken(new AccessTokenRequestOptions());

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            var missingToken = string.IsNullOrWhiteSpace(_accessToken);
            var expired = _accessTokenExpiresAt > DateTimeOffset.MinValue && _accessTokenExpiresAt <= DateTimeOffset.UtcNow;

            if (missingToken || expired)
            {
                if (!string.IsNullOrWhiteSpace(_refreshToken))
                {
                    var refreshed = await TryRefreshAccessTokenAsync();
                    if (refreshed)
                    {
                        var refreshedToken = new AccessToken
                        {
                            Value = _accessToken!,
                            Expires = _accessTokenExpiresAt
                        };

                        return new AccessTokenResult(AccessTokenResultStatus.Success, refreshedToken, null, null);
                    }
                }

                var current = _nav.ToBaseRelativePath(_nav.Uri);
                var returnUrl = "/" + current;
                var loginUrl = "/Account/Login?returnUrl=" + Uri.EscapeDataString(returnUrl);

                return new AccessTokenResult(
                    AccessTokenResultStatus.RequiresRedirect, null, loginUrl, null);
            }

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

                // expires_in is optional in a token response. Dereferencing it unconditionally would
                // throw into the catch below and report a successful refresh as a failure.
                _accessTokenExpiresAt = dto.ExpiresIn.HasValue
                                ? new DateTimeOffset(DateTime.SpecifyKind(dto.ExpiresIn.Value, DateTimeKind.Utc), TimeSpan.Zero)
                                : DateTimeOffset.UtcNow.AddMinutes(5);

                // Publish the refreshed token. Without this, anything reading "access_token" off
                // AuthenticationState keeps seeing the stale value for the rest of the session, and
                // NotifyAuthenticationStateChanged is never raised at all.
                Claim[] refreshedClaims = [
                    new Claim(ClaimTypes.NameIdentifier, _userId ?? string.Empty),
                                new Claim(ClaimTypes.Email, _email ?? string.Empty),
                                new Claim("access_token", _accessToken) ];

                _authenticationStateTask = Task.FromResult(
                    new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(refreshedClaims,
                        authenticationType: nameof(PersistentAuthenticationStateProvider)))));

                NotifyAuthenticationStateChanged(_authenticationStateTask);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}