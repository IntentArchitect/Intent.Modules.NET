using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Bugsnag.Templates.BugSnagQuartzJobListener
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BugSnagQuartzJobListenerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Bugsnag.BugSnagQuartzJobListener";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BugSnagQuartzJobListenerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Quartz")
                .AddUsing("Bugsnag")
                .AddUsing("Bugsnag.Payload")
                .AddClass($"BugSnagQuartzJobListener", @class =>
                {
                    @class.ImplementsInterface("IJobListener");
                    @class.AddConstructor(ctor => ctor.AddParameter("IClient", "client", param => param.IntroduceReadonlyField()));
                    @class.AddProperty("string", "Name", prop =>
                    {
                        prop.WithoutSetter();
                        prop.Getter.WithExpressionImplementation(@"""BugSnag""");
                    });
                    @class.AddMethod("Task", "JobToBeExecuted",
                        method => method.AddParameter("IJobExecutionContext", "context")
                            .AddOptionalCancellationTokenParameter(this)
                            .AddStatement("return Task.CompletedTask;"));
                    @class.AddMethod("Task", "JobExecutionVetoed",
                        method => method.AddParameter("IJobExecutionContext", "context")
                            .AddOptionalCancellationTokenParameter(this)
                            .AddStatement("return Task.CompletedTask;"));
                    @class.AddMethod("Task", "JobWasExecuted", method =>
                        method.AddParameter("IJobExecutionContext", "context")
                            .AddParameter("JobExecutionException" + (OutputTarget.GetProject().NullableEnabled ? "?" : ""), "jobException")
                            .AddOptionalCancellationTokenParameter(this)
                            .AddStatement(new CSharpIfStatement("jobException is not null")
                                .AddStatement("_client.Notify(jobException, HandledState.ForUnhandledException());"))
                            .AddStatement("return Task.CompletedTask;"));
                });
        }

        [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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