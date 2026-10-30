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

namespace Intent.Modules.Application.Mediator.Templates.UnhandledExceptionBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UnhandledExceptionBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.UnhandledExceptionBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UnhandledExceptionBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"UnhandledExceptionBehaviour", @class =>
                {
                    var appName = ExecutionContext.GetApplicationConfig().Name;

                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.ImplementsInterface(UseType($"Mediator.IPipelineBehavior<{tRequest}, {tResponse}>"));
                    // Qualified because the Infrastructure project can also import NServiceBus.IMessage (a global using).
                    @class.AddGenericTypeConstraint(tRequest, c => c.AddType("global::Mediator.IMessage"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType($"Microsoft.Extensions.Logging.ILogger<{@class.Name}<{tRequest}, {tResponse}>>"), "logger", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");
                        ctor.AddStatement(@"_logRequestPayload = configuration.GetValue<bool?>(""CqrsSettings:LogRequestPayload"") ?? false;");
                    });

                    @class.AddField("bool", "_logRequestPayload", cfg => cfg.PrivateReadOnly());

                    @class.AddMethod($"{UseType("System.Threading.Tasks.ValueTask")}<{tResponse}>", "Handle", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddParameter(tRequest, "request");
                        method.AddParameter(UseType($"Mediator.MessageHandlerDelegate<{tRequest}, {tResponse}>"), "next");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddTryBlock(tryBlock =>
                        {
                            tryBlock.AddStatement("return await next(request, cancellationToken);");
                        });
                        method.AddCatchBlock(catchBlock =>
                        {
                            catchBlock.WithExceptionType(UseType("System.Exception")).WithParameterName("ex");
                            catchBlock.AddStatement($"var requestName = typeof({tRequest}).Name;", s => s.SeparatedFromNext());
                            catchBlock.AddIfStatement("_logRequestPayload", logIf =>
                            {
                                logIf.AddStatement($@"_logger.LogError(ex, ""{appName} Request: Unhandled Exception for Request {{Name}} {{@Request}}"", requestName, request);");
                            });
                            catchBlock.AddElseStatement(@else =>
                            {
                                @else.AddStatement($@"_logger.LogError(ex, ""{appName} Request: Unhandled Exception for Request {{Name}}"", requestName);");
                            });
                            catchBlock.AddStatement("throw;");
                        });
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("CqrsSettings:LogRequestPayload", true);
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
