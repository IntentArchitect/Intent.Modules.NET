using System;
using System.Collections.Generic;
using Intent.Engine;
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
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Components")
                .AddUsing("Microsoft.AspNetCore.Components.Authorization")
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Components.WebAssembly.Authentication")
                .AddUsing("System")
                .AddUsing("System.Net.Http.Json")
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
                        f.PrivateReadOnly();
                        f.WithAssignment(new CSharpStatement("_defaultUnauthenticatedTask"));
                    });
                    @class.AddField("Uri?", "_identityUrl", p => p.PrivateReadOnly());

                    @class.AddField(UseType("System.Net.Http.HttpClient"), "_refreshClient", p => p.PrivateReadOnly().WithAssignment("new HttpClient()"));
                    @class.AddField("string?", "_accessToken", p => p.Private());
                    @class.AddField("string?", "_refreshToken", p => p.Private());
                    @class.AddField("DateTimeOffset", "_accessTokenExpiresAt", p => p.Private().WithAssignment("DateTimeOffset.MinValue"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("PersistentComponentState", "state");
                        ctor.AddParameter("NavigationManager", "nav", p => p.IntroduceReadonlyField());

                        ctor.AddIfStatement($"!state.TryTakeFromJson<{GetTypeName(UserInfoTemplate.TemplateId)}>(nameof(UserInfo), out var userInfo) || userInfo is null", @if => @if.AddReturn(""));

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
                            ifs.AddStatement("_refreshToken = userInfo.RefreshToken;");
                            ifs.AddIfStatement("userInfo.AccessTokenExpiresAt.HasValue", i => i.AddStatement("_accessTokenExpiresAt = userInfo.AccessTokenExpiresAt.Value;"));
                            ifs.AddIfStatement("!string.IsNullOrEmpty(userInfo.RefreshUrl)", i => i.AddStatement("_identityUrl = new Uri(userInfo.RefreshUrl, UriKind.Absolute);"));
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
                        method.Async().WithReturnType("ValueTask<AccessTokenResult>");
                        method.AddParameter("AccessTokenRequestOptions", "options");

                        method.AddStatements(@"var missingToken = string.IsNullOrWhiteSpace(_accessToken);
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
                var returnUrl = ""/"" + current;
                var loginUrl = $""/account/login?returnUrl={Uri.EscapeDataString(returnUrl)}"";

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
            };".ConvertToStatements());
                        method.AddStatement("return new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null);");


                    });
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

                // compute expiry
                _accessTokenExpiresAt = new DateTimeOffset(dto.ExpiresIn!.Value, TimeSpan.Zero);
                return true;
            }}
            catch
            {{
                return false;
            }}".ConvertToStatements());
                    });
                });
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