using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.UnhandledExceptionBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UnhandledExceptionBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.UnhandledExceptionBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UnhandledExceptionBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MediatR")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"UnhandledExceptionBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var TRequest);
                    @class.AddGenericParameter("TResponse", out var TResponse);
                    @class.AddGenericTypeConstraint(TRequest, c => c.AddType("notnull"));
                    @class.ImplementsInterface($"IPipelineBehavior<{TRequest}, {TResponse}>");
                    
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"ILogger<UnhandledExceptionBehaviour<{TRequest},{TResponse}>>", "logger", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        ctor.AddStatement(@"_logRequestPayload = configuration.GetValue<bool?>(""CqrsSettings:LogRequestPayload"") ?? false;");
                    });
                    
                    @class.AddField("bool", "_logRequestPayload", cfg => cfg.PrivateReadOnly());
                    
                    @class.AddMethod($"Task<{TResponse}>", "Handle", method =>
                    {
                        method.Async();
                        method.AddParameter(TRequest, "request");
                        method.AddParameter($"RequestHandlerDelegate<{TResponse}>", "next");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddTryBlock(t =>
                        {
                            var cancellationToken = Project.TryGetMaxNetAppVersion(out var version) &&
                                                    version.Major is <= 2 or > 6
                                ? "cancellationToken"
                                : string.Empty;
                            t.AddStatement($"return await next({cancellationToken});");
                        });
                        method.AddCatchBlock(c =>
                        {
                            c.WithExceptionType("Exception").WithParameterName("ex");
                            c.AddStatement($"var requestName = typeof({TRequest}).Name;");
                            
                            c.AddIfStatement("_logRequestPayload", logIf =>
                            {
                                logIf.AddStatement($@"_logger.LogError(ex, ""{ExecutionContext.GetApplicationConfig().Name} Request: Unhandled Exception for Request {{Name}} {{@Request}}"", requestName, request);");
                            });
                            c.AddElseStatement(@else =>
                            {
                                @else.AddStatement($@"_logger.LogError(ex, ""{ExecutionContext.GetApplicationConfig().Name} Request: Unhandled Exception for Request {{Name}}"", requestName);");
                            });
                            
                            c.AddStatement("throw;");
                        });
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("CqrsSettings:LogRequestPayload", true);
            
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(0)
                .ForConcern("MediatR")
                .HasDependency(this));
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