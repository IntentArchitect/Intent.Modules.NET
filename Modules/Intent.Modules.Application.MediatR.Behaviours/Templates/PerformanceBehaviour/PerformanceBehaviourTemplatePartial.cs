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

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.PerformanceBehaviour
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class PerformanceBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.PerformanceBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PerformanceBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"PerformanceBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var TRequest)
                    .AddGenericParameter("TResponse", out var TResponse)
                    .AddGenericTypeConstraint(TRequest, cfg => cfg.AddType("notnull"))
                    .ImplementsInterface(UseType($"MediatR.IPipelineBehavior<{TRequest}, {TResponse}>"))
                    .AddConstructor(ctor =>
                    {
                        ctor.AddObjectInitStatement("_timer", "new Stopwatch();");
                        ctor.AddParameter(UseType($"Microsoft.Extensions.Logging.ILogger<PerformanceBehaviour<{TRequest},{TResponse}>>"), "logger", cfg => cfg.IntroduceReadonlyField());
                        ctor.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService", param => param.IntroduceReadonlyField());
                    });

                    @class.AddField(UseType("System.Diagnostics.Stopwatch"), "_timer", cfg =>
                    {
                        cfg.PrivateReadOnly();
                    });

                    @class.AddMethod(UseType($"System.Threading.Tasks.Task<{TResponse}>"), "Handle", method =>
                    {
                        method.Async();

                        method.AddParameter("TRequest", "request")
                            .AddParameter($"RequestHandlerDelegate<{TResponse}>", "next")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddInvocationStatement("_timer.Start", cfg => cfg.SeparatedFromNext());
                        method.AddObjectInitStatement("var response", "await next();", cfg => cfg.SeparatedFromNext());
                        method.AddInvocationStatement("_timer.Stop", cfg => cfg.SeparatedFromNext());
                        method.AddObjectInitStatement("var elapsedMilliseconds", "_timer.ElapsedMilliseconds;", cfg => cfg.SeparatedFromNext());

                        method.AddIfStatement("elapsedMilliseconds > 500", @if =>
                        {
                            @if.AddObjectInitStatement("var requestName", "typeof(TRequest).Name;");
                            @if.AddObjectInitStatement("var userId", "_currentUserService.UserId;");
                            @if.AddObjectInitStatement("var userName", "_currentUserService.UserName;", cfg => cfg.SeparatedFromNext());

                            @if.AddInvocationStatement("_logger.LogWarning", cfg =>
                            {
                                cfg.AddArgument($"\"{ExecutionContext.GetApplicationConfig().Name} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@UserId}} {{@UserName}} {{@Request}}\"")
                                    .AddArgument("requestName")
                                    .AddArgument("elapsedMilliseconds")
                                    .AddArgument("userId")
                                    .AddArgument("userName")
                                    .AddArgument("request");
                            });
                        });

                        method.AddReturn("response");
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
        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(1)
                .ForConcern("MediatR")
                .RequiresUsingNamespaces("MediatR")
                .HasDependency(this));
        }
    }
}