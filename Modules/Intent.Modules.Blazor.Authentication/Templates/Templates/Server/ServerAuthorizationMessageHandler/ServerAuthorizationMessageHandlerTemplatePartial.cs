using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ServerAuthorizationMessageHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServerAuthorizationMessageHandlerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.ServerAuthorizationMessageHandlerTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServerAuthorizationMessageHandlerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Net.Http")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Linq")
                .AddClass($"ServerAuthorizationMessageHandler", @class =>
                {
                    AddNugetDependency(NugetPackages.MicrosoftAspNetCoreHttpAbstractions(outputTarget));
                    @class.WithBaseType("DelegatingHandler");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor", param =>
                        {
                            param.IntroduceReadonlyField();
                        });

                        // Mirrors AuthorizationMessageHandler.ConfigureHandler(authorizedUrls:) so the
                        // token is only ever attached to the APIs it was issued for.
                        ctor.AddParameter("string[]", "authorizedUrls", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task<HttpResponseMessage> ", "SendAsync", method =>
                    {
                        method.Override().Protected();

                        method.AddParameter("HttpRequestMessage", "request");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        // Read per call, never cached. This handler instance is shared across every
                        // request and every user for the lifetime of the handler chain, so holding on to
                        // a token here would leak it between users.
                        method.AddAssignmentStatement("var context", new CSharpStatement("_httpContextAccessor.HttpContext;"));
                        method.AddAssignmentStatement("var token", new CSharpStatement("context?.User?.Claims.FirstOrDefault(c => c.Type == \"access_token\")?.Value;"));
                        method.AddAssignmentStatement("var requestUrl", new CSharpStatement("request.RequestUri?.AbsoluteUri ?? string.Empty;"));

                        method.AddIfStatement("!string.IsNullOrEmpty(token) && _authorizedUrls.Any(url => requestUrl.StartsWith(url, StringComparison.OrdinalIgnoreCase))", @if =>
                        {
                            @if.AddStatement("request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(\"Bearer\", token);");
                        });

                        method.AddReturn("base.SendAsync(request, cancellationToken)");
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
            // The server half of the HTTP pipeline in EVERY render mode, not an Interactive Server
            // concern: WebAssembly and Auto applications also render on the server when prerendering,
            // and the server must not use the WebAssembly AuthorizationMessageHandler to do it.
            return base.CanRunTemplate();
        }
    }
}
