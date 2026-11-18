using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (Model.GetScheduling()?.DisallowConcurrentExecution() == true)
                    {
                        @class.AddAttribute(UseType("Quartz.DisallowConcurrentExecution"));
                    }

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface(UseType("Quartz.IJob"));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                    // Quartz 4 (net10.0 only) changed IJob.Execute to return ValueTask and take a CancellationToken.
                    // Async() alone turns a ValueTask return type back into Task, so the shape is chosen here.
                    @class.AddMethod("Task", "Execute", method =>
                    {
                        method.Async(asValueTask: UsesQuartz4);
                        method.AddParameter("IJobExecutionContext", "context");
                        if (UsesQuartz4)
                        {
                            method.AddOptionalCancellationTokenParameter(this);
                        }

                        if (model.PublishedCommand() == null)
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                            AddUsing("System");
                            method.AddStatement("// IntentInitialGen");
                            method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
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

                    var method = @class.FindMethod("Execute");
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                    method.AddStatement($"var command = new {GetTypeName(commandTemplate)}();");
                    method.AddStatement(UsesQuartz4
                        ? "await _mediator.Send(command, cancellationToken);"
                        : "await _mediator.Send(command);");
                });
        }

        private bool UsesQuartz4 => QuartzVersion.UsesQuartz4(OutputTarget);

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
