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

namespace Intent.Modules.Application.Wolverine.Templates.LoggingMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class LoggingMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.LoggingMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public LoggingMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var currentUserService = GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface");
            var appName = ExecutionContext.GetApplicationConfig().Name;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("LoggingMiddleware", @class =>
                {
                    @class.AddField("bool", "_logRequestPayload", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");
                        ctor.AddStatement(@"_logRequestPayload = configuration.GetValue<bool?>(""CqrsSettings:LogRequestPayload"") ?? false;");
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "BeforeAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(UseType("Wolverine.Envelope"), "envelope");
                        method.AddParameter(UseType("Microsoft.Extensions.Logging.ILogger"), "logger");
                        method.AddParameter(currentUserService, "currentUserService");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
                        method.AddStatement("await LogAsync(envelope.Message, logger, currentUserService);");
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "LogAsync", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("object", "request");
                        method.AddParameter(UseType("Microsoft.Extensions.Logging.ILogger"), "logger");
                        method.AddParameter(currentUserService, "currentUserService");

                        method.AddStatement("var requestName = request.GetType().Name;");
                        method.AddStatement("var user = await currentUserService.GetAsync();");
                        method.AddIfStatement("_logRequestPayload", @if =>
                        {
                            @if.AddStatement($@"logger.LogInformation(""{appName} Request: {{Name}} {{@UserId}} {{@UserName}} {{@Request}}"", requestName, user?.Id, user?.Name, request);");
                        });
                        method.AddElseStatement(@else =>
                        {
                            @else.AddStatement($@"logger.LogInformation(""{appName} Request: {{Name}} {{@UserId}} {{@UserName}}"", requestName, user?.Id, user?.Name);");
                        });
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