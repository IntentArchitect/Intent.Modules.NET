using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Hangfire.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Hangfire.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireJobs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HangfireJobsTemplate : CSharpTemplateBase<HangfireJobModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Hangfire.HangfireJobs";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HangfireJobsTemplate(IOutputTarget outputTarget, HangfireJobModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name.ToPascalCase()}", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    @class.AddMethod("Task", "ExecuteAsync", method =>
                    {
                        method.Async();

                        method.AddAttribute(UseType("Hangfire.AutomaticRetry"), attConfig =>
                        {
                            attConfig.AddArgument($"Attempts = {model.GetJobOptions().RetryAttempts()}");
                            attConfig.AddArgument($"OnAttemptsExceeded = AttemptsExceededAction.{model.GetJobOptions().OnAttemptsExceeded().Value}");
                        });

                        if (model.GetJobOptions().DisallowConcurrentExecution())
                        {
                            method.AddAttribute(UseType("Hangfire.DisableConcurrentExecution"), attConfig =>
                            {
                                attConfig.AddArgument(model.GetJobOptions().ConcurrentExecutionTimeout().ToString());
                            });
                        }

                        if (model.PublishedCommand() == null)
                        {
                            AddUsing("System");
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                            method.AddStatement("// IntentInitialGen");
                            method.AddStatement($"// TODO: Implement job functionality");
                            method.AddStatement($@"throw new {UseType("System.NotImplementedException")}(""Your implementation here..."");");
                        }
                    });
                })
                .OnBuild(file =>
                {
                    var command = Model.PublishedCommand();
                    if (command == null)
                    {
                        return;
                    }

                    // The command's own template decides the dispatcher: Mediator and MediatR both fill this role.
                    var commandTemplate = GetTemplate<ICSharpTemplate>(CommandContractRole, command);
                    var senderType = commandTemplate.Id == MediatorCommandModelsTemplateId
                        ? "Mediator.ISender"
                        : "MediatR.ISender";

                    var @class = file.Classes.First();
                    @class.Constructors.First().AddParameter(UseType(senderType), "mediator", param =>
                    {
                        param.IntroduceReadonlyField();
                    });

                    var method = @class.FindMethod("ExecuteAsync");
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithSignatureFully());
                    method.AddStatement($"var command = new {GetTypeName(commandTemplate)}();");
                    method.AddStatement("await _mediator.Send(command);");
                });
        }

        private const string CommandContractRole = "Application.Contract.Command";
        private const string MediatorCommandModelsTemplateId = "Intent.Application.Mediator.CommandModels";

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