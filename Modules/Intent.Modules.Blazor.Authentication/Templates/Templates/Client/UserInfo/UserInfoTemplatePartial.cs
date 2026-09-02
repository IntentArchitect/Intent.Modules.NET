using System;
using System.Collections.Generic;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Client.UserInfo
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UserInfoTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Client.UserInfoTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UserInfoTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // This DTO is embedded UNENCRYPTED in the prerendered HTML by PersistAsJson, and then lives
            // in WASM memory — readable by any XSS. So RefreshToken is only emitted for the JWT mode,
            // which is the only mode that still performs a browser-side refresh (against ASP.NET Core
            // Identity's "refresh" endpoint). OIDC deliberately ships no browser-side refresh, so
            // shipping its long-lived refresh token to the browser would be pure exposure with nothing
            // reading it. RefreshUrl travels with it — it is application configuration
            // (TokenEndpoint:Uri), identical for every user and request, so it only belongs on a
            // per-user session DTO for as long as the browser-side refresh consumes it.
            //
            // Deliberately NOT added: a ClientId property. An OAuth client_id is application config,
            // not user info; if something client-side ever needs it, the channel is the generated
            // <Client>/wwwroot/appsettings.json that Program.cs already loads.
            var isJwt = this.GetAuthenticationType().IsBearerTokenJWT();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddClass($"UserInfo", @class =>
                {
                    @class.AddProperty("string", "UserId", p => p.Required());
                    @class.AddProperty("string", "Email", p => p.Required());
                    @class.AddProperty("string?", "AccessToken");

                    if (isJwt)
                    {
                        @class.AddProperty("string?", "RefreshToken");
                        @class.AddProperty("string?", "RefreshUrl");
                    }

                    @class.AddProperty("DateTime?", "AccessTokenExpiresAt");
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