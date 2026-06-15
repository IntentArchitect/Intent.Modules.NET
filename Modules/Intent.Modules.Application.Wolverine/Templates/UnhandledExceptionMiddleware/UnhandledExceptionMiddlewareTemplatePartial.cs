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

namespace Intent.Modules.Application.Wolverine.Templates.UnhandledExceptionMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UnhandledExceptionMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.UnhandledExceptionMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UnhandledExceptionMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var appName = ExecutionContext.GetApplicationConfig().Name;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("UnhandledExceptionMiddleware", @class =>
                {
                    @class.AddField("bool", "_logRequestPayload", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");
                        ctor.AddStatement(@"_logRequestPayload = configuration.GetValue<bool?>(""CqrsSettings:LogRequestPayload"") ?? false;");
                    });

                    @class.AddMethod("void", "OnException", method =>
                    {
                        method.AddAttribute($"[{UseType("Wolverine.Attributes.WolverineOnException")}]");
                        method.AddParameter("Exception", "exception");
                        method.AddParameter(UseType("Wolverine.Envelope"), "envelope");
                        method.AddParameter(UseType("Microsoft.Extensions.Logging.ILogger"), "logger");
                        method.AddStatement("LogException(exception, envelope.Message, logger);");
                    });

                    @class.AddMethod("void", "LogException", method =>
                    {
                        method.Private();
                        method.AddParameter("Exception", "exception");
                        method.AddParameter("object", "request");
                        method.AddParameter(UseType("Microsoft.Extensions.Logging.ILogger"), "logger");

                        method.AddIfStatement($"exception is {UseType("FluentValidation.ValidationException")}", @if =>
                        {
                            @if.AddStatement("return;");
                        });

                        method.AddStatement("var requestName = request?.GetType().Name;");
                        method.AddIfStatement("_logRequestPayload", @if =>
                        {
                            @if.AddStatement($@"logger.LogError(exception, ""{appName} Request: Unhandled Exception for Request {{Name}} {{@Request}}"", requestName, request);");
                        });
                        method.AddElseStatement(@else =>
                        {
                            @else.AddStatement($@"logger.LogError(exception, ""{appName} Request: Unhandled Exception for Request {{Name}}"", requestName);");
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