using System;
using System.Collections.Generic;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Api;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.UserInfo;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.PersistingServerAuthenticationStateProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PersistingServerAuthenticationStateProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.PersistingServerAuthenticationStateProviderTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PersistingServerAuthenticationStateProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // The refresh_token / RefreshUrl half of the persisted UserInfo exists only to feed the
            // browser-side refresh in PersistentAuthenticationStateProvider, which is emitted for the
            // JWT mode only. For OIDC nothing reads it, and persisting it would ship a long-lived
            // refresh token unencrypted in the prerendered HTML for no benefit.
            var isJwt = this.GetAuthenticationType().IsBearerTokenJWT();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Components")
                .AddUsing("Microsoft.AspNetCore.Components.Authorization")
                .AddUsing("Microsoft.AspNetCore.Components.Server")
                .AddUsing("Microsoft.AspNetCore.Components.Web")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Threading")
                .AddUsing("System.Security.Claims")
                .AddUsing("System")
                .AddClass($"PersistingServerAuthenticationStateProvider", @class =>
                {
                    // Deliberately NOT an IAccessTokenProvider. That is a WebAssembly abstraction, and
                    // satisfying it here meant RequestAccessToken() calling GetAuthenticationStateAsync()
                    // outside a component scope - which throws - and reporting every token as never
                    // expiring. The server forwards its token through ServerAuthorizationMessageHandler,
                    // which reads the claim per request instead.
                    @class.WithBaseType("ServerAuthenticationStateProvider");
                    @class.ImplementsInterface("IDisposable");
                    @class.AddField("IdentityOptions", "options", f => f.PrivateReadOnly());
                    @class.AddField("PersistingComponentStateSubscription", "subscription", f => f.PrivateReadOnly());
                    @class.AddField("Task<AuthenticationState>?", "authenticationStateTask");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("PersistentComponentState", "persistentComponentState", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IOptions<IdentityOptions>", "optionsAccessor");

                        if (isJwt)
                        {
                            ctor.AddParameter(this.UseType("Microsoft.Extensions.Configuration.IConfiguration"), "config", p => p.IntroduceReadonlyField());
                        }

                        ctor.AddStatement("options = optionsAccessor.Value;");
                        ctor.AddStatement("AuthenticationStateChanged += OnAuthenticationStateChanged;");
                        ctor.AddStatement("subscription = _persistentComponentState.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);");
                    });

                    @class.AddMethod("void", "OnAuthenticationStateChanged", method =>
                    {
                        method.Private();
                        method.AddParameter("Task<AuthenticationState>", "task");
                        method.AddStatement("authenticationStateTask = task;");
                    });

                    @class.AddMethod("Task", "OnPersistingAsync", method =>
                    {
                        method.Async().Private();

                        // A provider constructed outside a render (e.g. by a handler chain built in its own
                        // DI scope) never receives an authentication state, and must not fault the persist
                        // callback when it is asked to persist one.
                        method.AddIfStatement("authenticationStateTask is null", @if =>
                        {
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("var authenticationState = await authenticationStateTask;");
                        method.AddStatement("var principal = authenticationState.User;");

                        method.AddIfStatement("principal.Identity?.IsAuthenticated == true", @if =>
                        {
                            @if.AddStatement("var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;");
                            @if.AddStatement("var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;");
                            @if.AddStatement("var accessToken = principal.FindFirst(\"access_token\")?.Value;");
                            @if.AddStatement("var expiresAtClaim = principal.FindFirst(\"expires_at\")?.Value;");

                            if (isJwt)
                            {
                                @if.AddStatement("var refreshToken = principal.FindFirst(\"refresh_token\")?.Value;");
                                @if.AddStatement("var refreshUrl = _config.GetValue<string?>(\"TokenEndpoint:Uri\");");
                            }

                            @if.AddIfStatement($"!DateTime.TryParse(expiresAtClaim, null, {UseType("System.Globalization.DateTimeStyles")}.RoundtripKind, out var expiresAt)", iif =>
                            {
                                iif.AddStatement("expiresAt = DateTime.UtcNow.AddHours(1);");
                            });

                            @if.AddIfStatement("userId != null && email != null", @iif =>
                            {
                                var refreshInitializers = isJwt
                                    ? @"
                        RefreshToken = refreshToken,
                        RefreshUrl = refreshUrl,"
                                    : string.Empty;

                                @iif.AddStatement($@"var userInfo = new UserInfo {{
                        UserId = userId,
                        Email = email,
                        AccessToken = accessToken,{refreshInitializers}
                        AccessTokenExpiresAt = expiresAt}};");
                                @iif.AddStatement($"_persistentComponentState.PersistAsJson(nameof({GetTypeName(UserInfoTemplate.TemplateId)}), userInfo);");
                            });
                        });
                    });

                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.AddStatement("subscription.Dispose();");
                        method.AddStatement("AuthenticationStateChanged -= OnAuthenticationStateChanged;");
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
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveWebAssembly();
        }
    }
}
