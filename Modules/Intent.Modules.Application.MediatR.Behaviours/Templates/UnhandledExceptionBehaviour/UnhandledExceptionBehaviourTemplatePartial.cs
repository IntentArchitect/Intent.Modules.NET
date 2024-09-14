using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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
                        ctor.AddParameter($"ILogger<{TRequest}>", "logger", param => param.IntroduceReadonlyField());
                    });
                    @class.AddMethod($"Task<{TResponse}>", "Handle", method =>
                    {
                        method.Async();
                        method.AddParameter(TRequest, "request");
                        method.AddParameter($"RequestHandlerDelegate<{TResponse}>", "next");
                        method.AddOptionalCancellationTokenParameter();
                        method.AddTryBlock(t =>
                        {
                            t.AddStatement("return await next();");
                        });
                        method.AddCatchBlock(c => c
                            .WithExceptionType("Exception")
                            .WithParameterName("ex")
                            .AddStatement($"var requestName = typeof({TRequest}).Name;")
                            .AddStatement(@"_logger.LogError(ex, ""CleanArchitecture Request: Unhandled Exception for Request {Name} {@Request}"", requestName, request);")
                            .AddStatement("throw;"));
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(0)
                .ForConcern("MediatR")
                .RequiresUsingNamespaces("MediatR")
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