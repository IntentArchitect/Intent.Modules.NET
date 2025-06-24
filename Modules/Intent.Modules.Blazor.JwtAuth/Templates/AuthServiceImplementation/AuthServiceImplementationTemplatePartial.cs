using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.JwtAuth.Templates.AuthServiceImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AuthServiceImplementationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.JwtAuth.AuthServiceImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AuthServiceImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AuthService", @class =>
                {
                    @class.WithBaseType(this.GetAuthServiceInterfaceName());
                    @class.AddField("string", "AuthTokenKey", field => field.PrivateConstant("\"authToken\""));
                    @class.AddField("string", "RefreshTokenKey", field => field.PrivateConstant("\"refreshToken\""));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("HttpClient", "httpClient", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("AuthenticationStateProvider", "authenticationStateProvider", param =>
                        {
                            @class.AddField(this.GetJwtAuthenticationStateProviderName(), "_authenticationStateProvider", field => field.PrivateReadOnly());
                            ctor.AddStatement($"_authenticationStateProvider = ({this.GetJwtAuthenticationStateProviderName()}){param.Name};");
                        });
                        ctor.AddParameter("IJSRuntime", "jsRuntime", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("NavigationManager", "navigationManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });

                        @class.AddField("Timer", "_tokenCheckTimer", field => field.PrivateReadOnly());
                        ctor.AddStatements(@"
        _tokenCheckTimer = new Timer(async _ =>
        {
            var isLoggedIn = await IsLoggedIn();
            if (!isLoggedIn)
            {
                return;
            }
            var token = await GetAccessTokenAsync();
            if (token == null)
            {
                await Logout();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Check every minute".ConvertToStatements());
                    });

                    @class.AddMethod("void", "Register", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "username");
                        method.AddParameter("string", "password");

                        method.AddStatements(@"
        var loginRequest = new { Email = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync(""api/Account/Register"", loginRequest);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.ReasonPhrase);
        }".ConvertToStatements());
                    });

                    @class.AddMethod("bool", "Login", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "username");
                        method.AddParameter("string", "password");

                        method.AddStatements(@"
        var loginRequest = new { Email = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync(""api/Account/Login"", loginRequest);

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        await StoreTokens(result.AuthenticationToken, result.RefreshToken);

        _authenticationStateProvider.NotifyUserAuthentication(result.AuthenticationToken);

        return true;".ConvertToStatements());
                    });

                    @class.AddMethod("string?", "GetAccessTokenAsync", method =>
                    {
                        method.Async();
                        method.AddStatements(@"
        var accessToken = await _jsRuntime.InvokeAsync<string>(""localStorage.getItem"", AuthTokenKey);

        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        if (IsTokenAboutToExpire(accessToken))
        {
            accessToken = await RefreshAccessTokenAsync();
        }

        return accessToken;".ConvertToStatements());
                    });

                    @class.AddMethod("void", "Logout", method =>
                    {
                        method.Async();
                        method.AddStatements(@"
        await _jsRuntime.InvokeVoidAsync(""localStorage.removeItem"", AuthTokenKey);
        await _jsRuntime.InvokeVoidAsync(""localStorage.removeItem"", RefreshTokenKey);
        _authenticationStateProvider.NotifyUserLogout();
        _navigationManager.NavigateTo($""Auth/Login?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}"", forceLoad: true);
        await _httpClient.PostAsJsonAsync(""api/Account/Logout"", new object());".ConvertToStatements());
                    });

                    @class.AddMethod("string", "RefreshAccessTokenAsync", method =>
                    {
                        method.Private().Async();
                        method.AddStatements(@"
        var refreshToken = await _jsRuntime.InvokeAsync<string>(""localStorage.getItem"", RefreshTokenKey);

        if (string.IsNullOrEmpty(refreshToken))
            return null;

        var refreshRequest = new { RefreshToken = refreshToken };
        var response = await _httpClient.PostAsJsonAsync(""api/Account/Refresh"", refreshRequest);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            await StoreTokens(result.AuthenticationToken, result.RefreshToken);
            _authenticationStateProvider.NotifyUserAuthentication(result.AuthenticationToken);
            return result.AuthenticationToken;
        }

        // On failure, logout
        await Logout();
        return null;".ConvertToStatements());
                    });

                    @class.AddMethod("bool", "IsLoggedIn", method =>
                    {
                        method.Async();
                        method.AddStatements(@"
        var accessToken = await _jsRuntime.InvokeAsync<string>(""localStorage.getItem"", AuthTokenKey);
        return !string.IsNullOrEmpty(accessToken);");
                    });

                    @class.AddMethod("void", "StoreTokens", method =>
                    {
                        method.Private().Async();
                        method.AddParameter("string", "accessToken");
                        method.AddParameter("string", "refreshToken");
                        method.AddStatements(@"
        await _jsRuntime.InvokeVoidAsync(""localStorage.setItem"", AuthTokenKey, accessToken);
        await _jsRuntime.InvokeVoidAsync(""localStorage.setItem"", RefreshTokenKey, refreshToken);");
                    });

                    @class.AddMethod("bool", "IsTokenAboutToExpire", method =>
                    {
                        method.Private();
                        method.AddParameter("string", "token");
                        method.AddStatements(@"
        var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(token);
        return jwt.ValidTo < DateTime.UtcNow.AddMinutes(-2); // Check if expiring within two minute".ConvertToStatements());
                    });

                    @class.AddNestedClass("TokenResponse", nestedClass =>
                    {
                        nestedClass.Private();
                        nestedClass.AddProperty("string", "AuthenticationToken");
                        nestedClass.AddProperty("string", "RefreshToken");
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
    }
}