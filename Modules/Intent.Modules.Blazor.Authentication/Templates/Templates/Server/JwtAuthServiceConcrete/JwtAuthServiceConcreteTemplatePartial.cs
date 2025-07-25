using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.JwtAuthServiceConcrete
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JwtAuthServiceConcreteTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.JwtAuthServiceConcreteTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JwtAuthServiceConcreteTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
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
                .AddUsing("Microsoft.AspNetCore.Authentication.Cookies")
                .AddClass($"JwtAuthService", @class =>
                {
                    @class.Internal();
                    @class.ImplementsInterface(GetTypeName(AuthServiceInterfaceTemplate.TemplateId));

                    @class.AddField("HttpClient", "_httpClient");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IHttpContextAccessor ", "httpContextAccessor", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"IHttpClientFactory", "httpClientFactory");
                        ctor.AddStatement("_httpClient = httpClientFactory.CreateClient(\"jwtClient\");");
                        ctor.AddParameter($"{GetTypeName(IdentityRedirectManagerTemplate.TemplateId)}", "redirectManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task<string>", "ConfirmEmail", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "userId");
                        method.AddParameter("string", "code");

                        method.AddAssignmentStatement("var response", new CSharpStatement("await _httpClient.PostAsJsonAsync(\"/confirmEmail\", new { UserId = userId, Code = code, changedEmail = false });"));

                        method.AddIfStatement("response.IsSuccessStatusCode", @if =>
                        {
                            @if.AddReturn("await response.Content.ReadAsStringAsync();");
                        });

                        method.AddReturn("\"Email could not be confirmed\"");
                    });

                    @class.AddMethod("Task", "ForgotPassword", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");

                        method.AddStatement("await _httpClient.PostAsJsonAsync(\"/forgotPassword\", new { Email = email });");
                        method.AddStatement("_redirectManager.RedirectTo(\"Account/ForgotPasswordConfirmation\");");
                    });

                    @class.AddMethod("Task", "Login", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "username");
                        method.AddParameter("string", "password");
                        method.AddParameter("bool", "rememberMe");
                        method.AddParameter("string", "returnUrl");

                        method.AddAssignmentStatement("var tokenResponse", new CSharpStatement("await _httpClient.PostAsJsonAsync(\"/login\", new { Email = username, Password = password });"));

                        method.AddIfStatement("tokenResponse.IsSuccessStatusCode", @if =>
                        {
                            @if.AddAssignmentStatement("var tokens", new CSharpStatement("await tokenResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();"));
                            @if.AddStatements(@"var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, username),
                                new Claim(ClaimTypes.Email, username),
                                new Claim(""access_token"", tokens.AccessToken),
                                new Claim(""refresh_token"", tokens.RefreshToken),
                                new Claim(""token_type"", tokens.TokenType),
                                new Claim(""expires_at"", tokens.ExpiresIn.ToString())
                            };".ConvertToStatements());
                            @if.AddAssignmentStatement("var claimsIdentity", new CSharpStatement("new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);"));
                            @if.AddAssignmentStatement("var claimsPrincipal", new CSharpStatement("new ClaimsPrincipal(claimsIdentity);"));
                            @if.AddStatement("await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimsPrincipal, new AuthenticationProperties { IsPersistent = rememberMe });");
                            @if.AddStatement("_redirectManager.RedirectTo(returnUrl);");
                        }).AddElseStatement(@else =>
                        {
                            @else.AddStatement("throw new Exception(\"Error: Invalid login attempt.\");");
                        });
                    });

                    @class.AddMethod("Task", "Register", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "password");
                        method.AddParameter("string", "returnUrl");

                        method.AddAssignmentStatement("var registerResponse", new CSharpStatement("await _httpClient.PostAsJsonAsync(\"/register\", new { Email = email, Password = password });"));
                        method.AddIfStatement("!registerResponse.IsSuccessStatusCode", @if =>
                        {
                            @if.AddStatement("throw new Exception(\"Registration failed\");");
                        });

                        method.AddStatement("_redirectManager.RedirectTo(\"Account/RegisterConfirmation\", new() { [\"email\"] = email, [\"returnUrl\"] = returnUrl });");
                    });


                    @class.AddMethod("Task", "ResendEmailConfirmation", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");

                        method.AddStatement("await _httpClient.PostAsJsonAsync(\"/resendConfirmationEmail\", new { Email = email });");
                    });

                    @class.AddMethod("Task", "ResetPassword", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "code");
                        method.AddParameter("string", "password");

                        method.AddAssignmentStatement("var response", new CSharpStatement("await _httpClient.PostAsJsonAsync(\"/resetPassword\", new { Email = email, ResetCode = code, NewPassword = password });"));

                        method.AddIfStatement("!response.IsSuccessStatusCode", @if =>
                        {
                            @if.AddStatement("throw new Exception(\"Reset password failed\");");
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
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetAuthenticationType().Authentication().IsJwt();
        }
    }
}