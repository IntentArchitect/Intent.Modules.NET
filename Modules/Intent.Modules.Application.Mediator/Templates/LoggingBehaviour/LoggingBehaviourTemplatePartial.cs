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

namespace Intent.Modules.Application.Mediator.Templates.LoggingBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class LoggingBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.LoggingBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public LoggingBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationBinder(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"LoggingBehaviour", @class =>
                {
                    var appName = ExecutionContext.GetApplicationConfig().Name;
                    var currentUserService = GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface", TemplateDiscoveryOptions.DoNotThrow);
                    var hasIdentity = !string.IsNullOrEmpty(currentUserService);

                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.WithBaseType(UseType($"Mediator.MessagePreProcessor<{tRequest}, {tResponse}>"));
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

                    @class.AddMethod(UseType("System.Threading.Tasks.ValueTask"), "Handle", method =>
                    {
                        method.Protected();
                        method.Override();
                        if (hasIdentity)
                        {
                            method.Async(asValueTask: true);
                        }
                        method.AddParameter(tRequest, "request");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddStatement($"var requestName = typeof({tRequest}).Name;", s =>
                        {
                            if (!hasIdentity)
                            {
                                s.SeparatedFromNext();
                            }
                        });
                        if (hasIdentity)
                        {
                            method.AddStatement("var user = await _currentUserService.GetAsync();", s => s.SeparatedFromNext());
                        }

                        method.AddIfStatement("_logRequestPayload", @if =>
                        {
                            @if.AddStatement(hasIdentity
                                ? $@"_logger.LogInformation(""{appName} Request: {{Name}} {{@UserId}} {{@UserName}} {{@Request}}"", requestName, user?.Id, user?.Name, request);"
                                : $@"_logger.LogInformation(""{appName} Request: {{Name}} {{@Request}}"", requestName, request);");
                        });
                        method.AddElseStatement(@else =>
                        {
                            @else.AddStatement(hasIdentity
                                ? $@"_logger.LogInformation(""{appName} Request: {{Name}} {{@UserId}} {{@UserName}}"", requestName, user?.Id, user?.Name);"
                                : $@"_logger.LogInformation(""{appName} Request: {{Name}}"", requestName);");
                        });

                        if (!hasIdentity)
                        {
                            method.AddStatement($"return {UseType("System.Threading.Tasks.ValueTask")}.CompletedTask;", s => s.SeparatedFromPrevious());
                        }
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
