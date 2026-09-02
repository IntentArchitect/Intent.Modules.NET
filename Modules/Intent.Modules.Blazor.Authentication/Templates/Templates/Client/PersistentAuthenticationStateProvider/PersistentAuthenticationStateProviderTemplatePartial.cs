using System;
using System.Collections.Generic;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Api;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.UserInfo;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Client.PersistentAuthenticationStateProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PersistentAuthenticationStateProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Client.PersistentAuthenticationStateProviderTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PersistentAuthenticationStateProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // Browser-side token refresh is emitted for the JWT mode ONLY.
            //
            // JWT: "refresh" is a genuine ASP.NET Core Identity endpoint and the refresh token is
            // already in the browser, so refreshing there costs nothing extra.
            //
            // OIDC: an OIDC provider has no "refresh" path — it expects a form-encoded
            // grant_type=refresh_token at connect/token, which needs a client_id in the browser and
            // therefore a public client registration plus browser-origin CORS. That forfeits exactly
            // the property that makes the server-side OIDC design better than a browser-side one. So
            // OIDC ships no refresh: RequestAccessToken falls straight through to the login redirect
            // on a missing or expired token. If refresh is wanted for OIDC later, the right shape is a
            // server-side endpoint that reads refresh_token from the auth cookie, calls the IdP with
            // client_id + client_secret, and re-issues the cookie.
            var isJwt = this.GetAuthenticationType().IsBearerTokenJWT();

            // Resolved from the modelled login page's Page.Route rather than hardcoded, so a user who
            // moves the page in the designer has this redirect follow them.
            var loginRoute = this.GetLoginRoute();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Components")
                .AddUsing("Microsoft.AspNetCore.Components.Authorization")
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Components.WebAssembly.Authentication")
                .AddUsing("System")
                .AddClass($"PersistentAuthenticationStateProvider", @class =>
                {
                    @class.WithBaseType("AuthenticationStateProvider");
                    @class.ImplementsInterface("IAccessTokenProvider");
                    @class.AddField("Task<AuthenticationState>", "_defaultUnauthenticatedTask", f =>
                    {
                        f.PrivateReadOnly().Static();
                        f.WithAssignment(new CSharpStatement("Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))"));
                    });
                    @class.AddField("Task<AuthenticationState>", "_authenticationStateTask", f =>
                    {
                        // JWT reassigns this after a successful refresh so that
                        // NotifyAuthenticationStateChanged hands out the new access token. Nothing
                        // reassigns it in the other modes, so it stays readonly there.
                        if (isJwt)
                        {
                            f.Private();
                        }
                        else
                        {
                            f.PrivateReadOnly();
                        }

                        f.WithAssignment(new CSharpStatement("_defaultUnauthenticatedTask"));
                    });
                    @class.AddField("string?", "_accessToken", p => p.Private());
                    @class.AddField("DateTimeOffset", "_accessTokenExpiresAt", p => p.Private().WithAssignment(new CSharpStatement("DateTimeOffset.MinValue")));

                    if (isJwt)
                    {
                        CSharpFile.AddUsing("System.Net.Http.Json");

                        // Retained so a refreshed access token can be republished on the same identity.
                        @class.AddField("string?", "_userId", p => p.Private());
                        @class.AddField("string?", "_email", p => p.Private());
                        @class.AddField("Uri?", "_identityUrl", p => p.Private());
                        @class.AddField(UseType("System.Net.Http.HttpClient"), "_refreshClient", p => p.PrivateReadOnly().WithAssignment(new CSharpStatement("new HttpClient()")));
                        @class.AddField("string?", "_refreshToken", p => p.Private());
                    }

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("PersistentComponentState", "state");
                        ctor.AddParameter("NavigationManager", "nav", p => p.IntroduceReadonlyField());

                        ctor.AddIfStatement($"!state.TryTakeFromJson<{GetTypeName(UserInfoTemplate.TemplateId)}>(nameof(UserInfo), out var userInfo) || userInfo is null", @if => @if.AddReturn(""));

                        if (isJwt)
                        {
                            ctor.AddStatement("_userId = userInfo.UserId;");
                            ctor.AddStatement("_email = userInfo.Email;");
                        }

                        ctor.AddStatements(@"Claim[] claims = [
                            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                            new Claim(ClaimTypes.Email, userInfo.Email),
                            new Claim(""access_token"", userInfo.AccessToken == null ? """" : userInfo.AccessToken) ];".ConvertToStatements());

                        ctor.AddStatements(@"_authenticationStateTask = Task.FromResult(
                            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                                authenticationType: nameof(PersistentAuthenticationStateProvider)))));".ConvertToStatements());

                        ctor.AddIfStatement("!string.IsNullOrWhiteSpace(userInfo.AccessToken)", ifs =>
                        {
                            ifs.AddStatement("_accessToken = userInfo.AccessToken;");
                            ifs.AddIfStatement("userInfo.AccessTokenExpiresAt.HasValue", i => i.AddStatement("_accessTokenExpiresAt = userInfo.AccessTokenExpiresAt.Value;"));

                            if (isJwt)
                            {
                                ifs.AddStatement("_refreshToken = userInfo.RefreshToken;");
                                ifs.AddIfStatement("!string.IsNullOrEmpty(userInfo.RefreshUrl)", i => i.AddStatement("_identityUrl = new Uri(userInfo.RefreshUrl, UriKind.Absolute);"));
                            }
                        });
                    });

                    @class.AddMethod("Task<AuthenticationState>", "GetAuthenticationStateAsync", method => method.Override().AddReturn("_authenticationStateTask"));

                    @class.AddMethod("ValueTask<AccessTokenResult>", "RequestAccessToken", method =>
                    {
                        method.WithReturnType("ValueTask<AccessTokenResult>");
                        method.WithExpressionBody("RequestAccessToken(new AccessTokenRequestOptions())");
                    });

                    @class.AddMethod("ValueTask<AccessTokenResult>", "RequestAccessToken", method =>
                    {
                        // Only the JWT variant awaits anything (the refresh call). Marking the OIDC
                        // variant async would emit a CS1998 warning into every generated application,
                        // so its returns are wrapped in ValueTask.FromResult instead.
                        if (isJwt)
                        {
                            method.Async();
                        }

                        method.WithReturnType("ValueTask<AccessTokenResult>");
                        method.AddParameter("AccessTokenRequestOptions", "options");

                        method.AddStatements(@"var missingToken = string.IsNullOrWhiteSpace(_accessToken);
            var expired = _accessTokenExpiresAt > DateTimeOffset.MinValue && _accessTokenExpiresAt <= DateTimeOffset.UtcNow;".ConvertToStatements());

                        method.AddIfStatement("missingToken || expired", @if =>
                        {
                            if (isJwt)
                            {
                                @if.AddIfStatement("!string.IsNullOrWhiteSpace(_refreshToken)", refreshIf =>
                                {
                                    refreshIf.AddStatement("var refreshed = await TryRefreshAccessTokenAsync();");
                                    refreshIf.AddIfStatement("refreshed", ok =>
                                    {
                                        ok.AddStatements(@"var refreshedToken = new AccessToken
                    {
                        Value = _accessToken!,
                        Expires = _accessTokenExpiresAt
                    };".ConvertToStatements());
                                        ok.AddReturn(WrapReturn(isJwt, "new AccessTokenResult(AccessTokenResultStatus.Success, refreshedToken, null, null)"));
                                    });
                                });
                            }

                            // The IAccessTokenProvider contract is to RETURN the status and URL and let
                            // the caller navigate (AuthorizationMessageHandler →
                            // AccessTokenNotAvailableException → .Redirect()). Navigating inline here
                            // would tear the page out from under in-flight requests.
                            @if.AddStatements($@"var current = _nav.ToBaseRelativePath(_nav.Uri);
                var returnUrl = ""/"" + current;
                var loginUrl = ""{loginRoute}?returnUrl="" + Uri.EscapeDataString(returnUrl);".ConvertToStatements());
                            @if.AddReturn(WrapReturn(isJwt, "new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, loginUrl, null)"));
                        });

                        method.AddStatements(@"var expires = _accessTokenExpiresAt > DateTimeOffset.MinValue
                ? _accessTokenExpiresAt
                : DateTimeOffset.UtcNow.AddMinutes(5);

            var accessToken = new AccessToken
            {
                Value = _accessToken!,
                Expires = expires
            };".ConvertToStatements());
                        method.AddReturn(WrapReturn(isJwt, "new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null)"));
                    });

                    if (isJwt)
                    {
                        @class.AddMethod("Task<bool>", "TryRefreshAccessTokenAsync", method =>
                        {
                            method.Private().Async();
                            method.AddStatements($@"if (string.IsNullOrWhiteSpace(_refreshToken) || _identityUrl == null)
                return false;

            try
            {{
                var refreshUri = new Uri(_identityUrl, ""refresh""); // e.g. https://ids.example.com/refresh

                var response = await _refreshClient.PostAsJsonAsync(refreshUri, new
                {{
                    refreshToken = _refreshToken
                }});

                if (!response.IsSuccessStatusCode)
                    return false;

                var dto = await response.Content.ReadFromJsonAsync<{this.GetAccessTokenResponseTemplateName()}>();
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

                // Publish the refreshed token. Without this, anything reading ""access_token"" off
                // AuthenticationState keeps seeing the stale value for the rest of the session, and
                // NotifyAuthenticationStateChanged is never raised at all.
                Claim[] refreshedClaims = [
                    new Claim(ClaimTypes.NameIdentifier, _userId ?? string.Empty),
                    new Claim(ClaimTypes.Email, _email ?? string.Empty),
                    new Claim(""access_token"", _accessToken) ];

                _authenticationStateTask = Task.FromResult(
                    new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(refreshedClaims,
                        authenticationType: nameof(PersistentAuthenticationStateProvider)))));

                NotifyAuthenticationStateChanged(_authenticationStateTask);

                return true;
            }}
            catch
            {{
                return false;
            }}".ConvertToStatements());
                        });
                    }
                });
        }

        /// <summary>
        /// The JWT variant of the generated <c>RequestAccessToken</c> is <c>async</c>, so it returns
        /// its value directly; the OIDC variant is not, so its returns must be wrapped in a
        /// <c>ValueTask</c>.
        /// </summary>
        private static string WrapReturn(bool isJwt, string expression)
        {
            return isJwt ? expression : $"ValueTask.FromResult({expression})";
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer();
        }
    }
}
