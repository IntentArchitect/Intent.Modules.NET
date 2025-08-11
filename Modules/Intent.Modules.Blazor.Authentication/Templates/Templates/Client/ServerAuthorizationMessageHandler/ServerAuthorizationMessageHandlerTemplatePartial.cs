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

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Client.ServerAuthorizationMessageHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServerAuthorizationMessageHandlerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Client.ServerAuthorizationMessageHandlerTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServerAuthorizationMessageHandlerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
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
                    });

                    @class.AddMethod("Task<HttpResponseMessage> ", "SendAsync", method =>
                    {
                        method.Override().Protected();

                        method.AddParameter("HttpRequestMessage", "request");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddAssignmentStatement("var context", new CSharpStatement("_httpContextAccessor.HttpContext;"));
                        method.AddAssignmentStatement("var token", new CSharpStatement("context?.User?.Claims.FirstOrDefault(c => c.Type == \"access_token\")?.Value;"));

                        method.AddIfStatement("!string.IsNullOrEmpty(token)", @if =>
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
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer();
        }
    }
}