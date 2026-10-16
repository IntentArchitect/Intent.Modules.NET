using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.Mediator.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Templates.PerformanceBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PerformanceBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.PerformanceBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PerformanceBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"PerformanceBehaviour", @class =>
                {
                    var appName = ExecutionContext.GetApplicationConfig().Name;
                    var currentUserService = GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface", TemplateDiscoveryOptions.DoNotThrow);
                    var hasIdentity = !string.IsNullOrEmpty(currentUserService);

                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.ImplementsInterface(UseType($"Mediator.IPipelineBehavior<{tRequest}, {tResponse}>"));
                    @class.AddGenericTypeConstraint(tRequest, c => c.AddType(UseType("Mediator.IMessage")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType($"Microsoft.Extensions.Logging.ILogger<{@class.Name}<{tRequest}, {tResponse}>>"), "logger", param => param.IntroduceReadonlyField());
                        if (hasIdentity)
                        {
                            ctor.AddParameter(currentUserService, "currentUserService", param => param.IntroduceReadonlyField());
                        }
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

                        var stopwatch = UseType("System.Diagnostics.Stopwatch");
                        method.AddStatement($"var started = {stopwatch}.GetTimestamp();", s => s.SeparatedFromNext());
                        method.AddStatement("var response = await next(request, cancellationToken);", s => s.SeparatedFromNext());
                        method.AddStatement($"var elapsedMilliseconds = (long){stopwatch}.GetElapsedTime(started).TotalMilliseconds;", s => s.SeparatedFromNext());

                        method.AddIfStatement("elapsedMilliseconds > 500", elapsedIf =>
                        {
                            elapsedIf.AddStatement($"var requestName = typeof({tRequest}).Name;", s =>
                            {
                                if (!hasIdentity)
                                {
                                    s.SeparatedFromNext();
                                }
                            });
                            if (hasIdentity)
                            {
                                elapsedIf.AddStatement("var user = await _currentUserService.GetAsync();", s => s.SeparatedFromNext());
                            }

                            elapsedIf.AddIfStatement("_logRequestPayload", logIf =>
                            {
                                logIf.AddStatement(hasIdentity
                                    ? $@"_logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@UserId}} {{@UserName}} {{@Request}}"", requestName, elapsedMilliseconds, user?.Id, user?.Name, request);"
                                    : $@"_logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@Request}}"", requestName, elapsedMilliseconds, request);");
                            });
                            elapsedIf.AddElseStatement(@else =>
                            {
                                @else.AddStatement(hasIdentity
                                    ? $@"_logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds) {{@UserId}} {{@UserName}}"", requestName, elapsedMilliseconds, user?.Id, user?.Name);"
                                    : $@"_logger.LogWarning(""{appName} Long Running Request: {{Name}} ({{ElapsedMilliseconds}} milliseconds)"", requestName, elapsedMilliseconds);");
                            });
                        });

                        method.AddReturn("response");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetMediatorSettings().EnablePerformanceBehaviour();
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate())
            {
                return;
            }

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
