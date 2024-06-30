using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.QuartzScheduler.Api;
using Intent.QuartzScheduler.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.QuartzScheduler.Templates.ScheduledJob
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ScheduledJobTemplate : CSharpTemplateBase<ScheduledJobModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.QuartzScheduler.ScheduledJob";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ScheduledJobTemplate(IOutputTarget outputTarget, ScheduledJobModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Quartz")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name.ToPascalCase()}", @class =>
                {
                    if (Model.GetScheduling().DisallowConcurrentExecution())
                    {
                        @class.AddAttribute(UseType("Quartz.DisallowConcurrentExecution"));
                    }

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface(UseType("Quartz.IJob"));
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
                    @class.AddMethod("Task", "Execute", async method =>
                    {
                        method.Async();
                        method.AddParameter("IJobExecutionContext", "context");
                        if (model.PublishedCommand() != null)
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithSignatureFully());
                            method.AddStatement($"var command = new {GetTypeName("Application.Contract.Command", model.PublishedCommand())}();");
                            method.AddStatement("await _mediator.Send(command);");
                        }
                        else
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                            AddUsing("System");
                            method.AddStatement($@"throw new NotImplementedException(""Your implementation here..."");");
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