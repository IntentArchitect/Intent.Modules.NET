using System;
using System.Collections.Generic;
using Intent.Engine;
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
        public const string TemplateId = "Intent.Modules.Hangfire.HangfireJobs";

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
                        if (model.PublishedCommand() != null)
                        {
                            AddUsing("MediatR");
                            ctor.AddParameter("ISender", "mediator", param =>
                            {
                                param.IntroduceReadonlyField();
                            });
                        }
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

                        if (model.PublishedCommand() != null)
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithSignatureFully());
                            method.AddStatement($"var command = new {GetTypeName("Application.Contract.Command", model.PublishedCommand())}();");
                            method.AddStatement("await _mediator.Send(command);");
                        }
                        else
                        {
                            AddUsing("System");
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                            method.AddStatement($"// TODO: Implement job functionality");
                            method.AddStatement($@"throw new {UseType("System.NotImplementedException")}(""Your implementation here..."");");
                        }


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