using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.OidcAuthenticationOptions;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.OidcAuthServiceConcrete
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OidcAuthServiceConcreteTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.OidcAuthServiceConcreteTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OidcAuthServiceConcreteTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Authentication")
                .AddUsing("System.Net.Http")
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Net.Http.Json")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Security.Claims")
                .AddUsing("Microsoft.Extensions.Options")
                .AddClass($"OidcAuthService", @class =>
                {
                    @class.Internal();
                    @class.ImplementsInterface(GetTypeName(AuthServiceInterfaceTemplate.TemplateId));

                    @class.AddField("HttpClient", "_httpClient");
                    @class.AddField($"{GetTypeName(OidcAuthenticationOptionsTemplate.TemplateId)}", "_oidcAuthOptions");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IHttpContextAccessor ", "httpContextAccessor", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"IHttpClientFactory", "httpClientFactory");
                        ctor.AddStatement("_httpClient = httpClientFactory.CreateClient(\"oidcClient\");");
                        ctor.AddParameter($"{GetTypeName(IdentityRedirectManagerTemplate.TemplateId)}", "redirectManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"IOptions<{GetTypeName(OidcAuthenticationOptionsTemplate.TemplateId)}>", "oidcAuthOptions");
                        ctor.AddStatement("_oidcAuthOptions = oidcAuthOptions.Value;");
                    });

                    @class.AddMethod("Task", "Login", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "username");
                        method.AddParameter("string", "password");
                        method.AddParameter("bool", "rememberMe");
                        method.AddParameter("string", "returnUrl");

                        method.AddStatements(@"var tokenRequest = new Dictionary<string, string>
                        {
                            { ""grant_type"", ""password"" },
                            { ""client_id"", _oidcAuthOptions.ClientId },
                            { ""client_secret"", _oidcAuthOptions.ClientSecret },
                            { ""username"", username },
                            { ""password"", password },
                            { ""scope"", _oidcAuthOptions.DefaultScopes }
                        };".ConvertToStatements());

                        method.AddAssignmentStatement("var tokenResponse", new CSharpStatement("await _httpClient.PostAsJsonAsync(\"/connect/token\", new FormUrlEncodedContent(tokenRequest));"));

                        method.AddIfStatement("tokenResponse.IsSuccessStatusCode", @if =>
                        {
                            @if.AddAssignmentStatement("var tokens", new CSharpStatement("await tokenResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();"));
                            @if.AddStatements(@"var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, username),
                                new Claim(ClaimTypes.Email, username),
                                new Claim(""access_token"", tokens.AccessToken),
                                new Claim(""refresh_token"", tokens.RefreshToken)
                            };".ConvertToStatements());
                            @if.AddAssignmentStatement("var claimsIdentity", new CSharpStatement("new ClaimsIdentity(claims);"));
                            @if.AddAssignmentStatement("var claimsPrincipal", new CSharpStatement("new ClaimsPrincipal(claimsIdentity);"));
                            @if.AddStatement("await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties { IsPersistent = rememberMe });");
                            @if.AddStatement("_redirectManager.RedirectTo(returnUrl);");
                        }).AddElseStatement(@else =>
                        {
                            @else.AddStatement("throw new Exception(\"Error: Invalid login attempt.\");");
                        });
                    });

                    @class.AddNestedClass("AccessTokenResponse", nested =>
                    {
                        nested.AddProperty("string", "AccessToken");
                        nested.AddProperty("string", "RefreshToken");
                        nested.AddProperty("string", "TokenType");
                        nested.AddProperty("DateTime", "ExpiresIn");
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
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().Authentication().IsOidc();
        }
    }
}