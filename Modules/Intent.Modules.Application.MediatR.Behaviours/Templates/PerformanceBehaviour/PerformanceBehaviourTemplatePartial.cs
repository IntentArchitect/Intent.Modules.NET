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
                    .ImplementsInterface(UseType($"MediatR.IPipelineBehavior<{TRequest}, {TResponse}>"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddObjectInitStatement("_timer", "new Stopwatch();");
                        ctor.AddParameter(UseType($"Microsoft.Extensions.Logging.ILogger<PerformanceBehaviour<{TRequest},{TResponse}>>"), "logger",
                            cfg => cfg.IntroduceReadonlyField());
                        ctor.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        ctor.AddStatement(@"_logRequestPayload = configuration.GetValue<bool?>(""CqrsSettings:LogRequestPayload"") ?? false;");
                    });

                    @class.AddField(UseType("System.Diagnostics.Stopwatch"), "_timer", cfg => cfg.PrivateReadOnly());
                    @class.AddField("bool", "_logRequestPayload", cfg => cfg.PrivateReadOnly());

                    @class.AddMethod(UseType($"System.Threading.Tasks.Task<{TResponse}>"), "Handle", method =>
                    {
                        method.Async();

                        method.AddParameter("TRequest", "request")
                            .AddParameter($"RequestHandlerDelegate<{TResponse}>", "next")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddInvocationStatement("_timer.Start", cfg => cfg.SeparatedFromNext());

                        var cancellationToken = Project.TryGetMaxNetAppVersion(out var version) &&
                                                version.Major is <= 2 or > 6
                            ? "cancellationToken"
                            : string.Empty;
                        method.AddObjectInitStatement("var response", $"await next({cancellationToken});", cfg => cfg.SeparatedFromNext());
                        method.AddInvocationStatement("_timer.Stop", cfg => cfg.SeparatedFromNext());
                        method.AddObjectInitStatement("var elapsedMilliseconds", "_timer.ElapsedMilliseconds;", cfg => cfg.SeparatedFromNext());

                        method.AddIfStatement("elapsedMilliseconds > 500", elapsedIf =>
                        {
                            elapsedIf.AddObjectInitStatement("var requestName", "typeof(TRequest).Name;");
                            elapsedIf.AddObjectInitStatement("var user", "await _currentUserService.GetAsync();", cfg => cfg.SeparatedFromNext());

                            elapsedIf.AddIfStatement("_logRequestPayload", logIf =>
                            {
                                logIf.AddInvocationStatement("_logger.LogWarning", cfg =>
                                {
                                    cfg.AddArgument($"\"{ExecutionContext.GetApplicationConfig().Name} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@UserId}} {{@UserName}} {{@Request}}\"")
                                        .AddArgument("requestName")
                                        .AddArgument("elapsedMilliseconds")
                                        .AddArgument("user?.Id")
                                        .AddArgument("user?.Name")
                                        .AddArgument("request");
                                });
                            });
                            elapsedIf.AddElseStatement(@else =>
                            {
                                @else.AddInvocationStatement("_logger.LogWarning", cfg =>
                                {
                                    cfg.AddArgument($"\"{ExecutionContext.GetApplicationConfig().Name} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@UserId}} {{@UserName}}\"")
                                        .AddArgument("requestName")
                                        .AddArgument("elapsedMilliseconds")
                                        .AddArgument("user?.Id")
                                        .AddArgument("user?.Name");
                                });
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
            this.ApplyAppSetting("CqrsSettings:LogRequestPayload", true);
            
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(1)
                .ForConcern("MediatR")
                .HasDependency(this));
        }
    }
}