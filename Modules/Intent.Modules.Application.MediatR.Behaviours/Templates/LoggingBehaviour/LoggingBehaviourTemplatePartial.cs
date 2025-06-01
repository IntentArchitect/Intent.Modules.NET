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

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.LoggingBehaviour
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class LoggingBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.LoggingBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public LoggingBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsLogging(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"LoggingBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var TRequest);
                    @class.AddGenericTypeConstraint(TRequest, c => c.AddType("notnull"));
                    @class.ImplementsInterface(UseType($"MediatR.Pipeline.IRequestPreProcessor<{TRequest}>"));
                    
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType($"Microsoft.Extensions.Logging.ILogger<LoggingBehaviour<{TRequest}>>"), "logger", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration");

                        ctor.AddStatement(@"_logRequestPayload = configuration.GetValue<bool?>(""CqrsSettings:LogRequestPayload"") ?? false;");
                    });

                    @class.AddField("bool", "_logRequestPayload", cfg => cfg.PrivateReadOnly());

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "Process", method =>
                    {
                        method.AddParameter("TRequest", "request")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddObjectInitStatement("var requestName", "typeof(TRequest).Name;");
                        method.AddObjectInitStatement("var userId", "_currentUserService.UserId;");
                        method.AddObjectInitStatement("var userName", "_currentUserService.UserName;", cfg => cfg.SeparatedFromNext());

                        method.AddIfStatement("_logRequestPayload", logIf =>
                        {
                            logIf.AddInvocationStatement("_logger.LogInformation", cfg =>
                            {
                                cfg.AddArgument($"\"{ExecutionContext.GetApplicationConfig().Name} Request: {{Name}} {{@UserId}} {{@UserName}} {{@Request}}\"")
                                    .AddArgument("requestName")
                                    .AddArgument("userId")
                                    .AddArgument("userName")
                                    .AddArgument("request");
                            });
                        });
                        method.AddElseStatement(@else =>
                        {
                            @else.AddInvocationStatement("_logger.LogInformation", cfg =>
                            {
                                cfg.AddArgument($"\"{ExecutionContext.GetApplicationConfig().Name} Request: {{Name}} {{@UserId}} {{@UserName}}\"")
                                    .AddArgument("requestName")
                                    .AddArgument("userId")
                                    .AddArgument("userName");
                            });
                        });
                        
                        method.AddReturn("Task.CompletedTask");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("CqrsSettings:LogRequestPayload", true);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"LoggingBehaviour",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}