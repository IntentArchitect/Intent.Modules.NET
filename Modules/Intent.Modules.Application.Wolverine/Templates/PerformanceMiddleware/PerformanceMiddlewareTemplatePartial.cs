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

namespace Intent.Modules.Application.Wolverine.Templates.PerformanceMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PerformanceMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.PerformanceMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PerformanceMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var appName = ExecutionContext.GetApplicationConfig().Name;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("PerformanceMiddleware", @class =>
                {
                    var currentUserService = GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface", TemplateDiscoveryOptions.DoNotThrow);
                    var hasIdentity = !string.IsNullOrEmpty(currentUserService);

                    @class.AddField("long", "_longRunningThresholdMilliseconds", f => f.PrivateReadOnly().WithAssignment(new CSharpStatement("500")));
                    @class.AddField("bool", "_logRequestPayload", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");
                        ctor.AddStatement(@"_logRequestPayload = configuration.GetValue<bool?>(""CqrsSettings:LogRequestPayload"") ?? false;");
                    });

                    @class.AddMethod(UseType("System.Diagnostics.Stopwatch"), "Before", method =>
                    {
                        method.AddParameter(UseType("Wolverine.Envelope"), "envelope");
                        method.AddStatement("var stopwatch = new Stopwatch();");
                        method.AddStatement("stopwatch.Start();");
                        method.AddStatement("return stopwatch;");
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "FinallyAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(UseType("System.Diagnostics.Stopwatch"), "stopwatch");
                        method.AddParameter(UseType("Wolverine.Envelope"), "envelope");
                        method.AddParameter(UseType("Microsoft.Extensions.Logging.ILogger"), "logger");
                        if (hasIdentity)
                        {
                            method.AddParameter(currentUserService, "currentUserService");
                        }
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddStatement("stopwatch.Stop();");
                        method.AddIfStatement("stopwatch.ElapsedMilliseconds <= _longRunningThresholdMilliseconds", @if =>
                        {
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("var requestName = envelope.Message?.GetType().Name;");
                        if (hasIdentity)
                        {
                            method.AddStatement("var user = await currentUserService.GetAsync();");
                            method.AddIfStatement("_logRequestPayload", @if =>
                            {
                                @if.AddStatement($@"logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@UserId}} {{@UserName}} {{@Request}}"", requestName, stopwatch.ElapsedMilliseconds, user?.Id, user?.Name, envelope.Message);");
                            });
                            method.AddElseStatement(@else =>
                            {
                                @else.AddStatement($@"logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@UserId}} {{@UserName}}"", requestName, stopwatch.ElapsedMilliseconds, user?.Id, user?.Name);");
                            });
                        }
                        else
                        {
                            method.AddIfStatement("_logRequestPayload", @if =>
                            {
                                @if.AddStatement($@"logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@Request}}"", requestName, stopwatch.ElapsedMilliseconds, envelope.Message);");
                            });
                            method.AddElseStatement(@else =>
                            {
                                @else.AddStatement($@"logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds)"", requestName, stopwatch.ElapsedMilliseconds);");
                            });
                        }
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