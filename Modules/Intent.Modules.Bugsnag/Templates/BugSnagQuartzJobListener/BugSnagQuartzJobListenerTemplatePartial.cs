using System;
using System.Collections.Generic;
using System.Linq;
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
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Quartz")
                .AddUsing("Bugsnag")
                .AddUsing("Bugsnag.Payload")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"BugSnagQuartzJobListener", @class =>
                {
                    @class.ImplementsInterface("IJobListener");
                    @class.AddConstructor(ctor => ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField()));
                    @class.AddProperty("string", "Name", prop =>
                    {
                        prop.WithoutSetter();
                        prop.Getter.WithExpressionImplementation(@"""BugSnag""");
                    });
                })
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var usesQuartz4 = TryGetTemplate<ICSharpFileBuilderTemplate>(QuartzConfigurationRole, out var quartzConfiguration) &&
                        QuartzVersion.UsesQuartz4(quartzConfiguration);

                    // Quartz 4 changed the listener callbacks to return ValueTask and gave every IJobListener member a
                    // default implementation, so only the callback we use is emitted. A Task-returning callback
                    // compiles against Quartz 4 but is rejected at startup.
                    if (!usesQuartz4)
                    {
                        @class.AddMethod("Task", "JobToBeExecuted",
                            method => method.AddParameter("IJobExecutionContext", "context")
                                .AddOptionalCancellationTokenParameter(this)
                                .AddStatement("return Task.CompletedTask;"));
                        @class.AddMethod("Task", "JobExecutionVetoed",
                            method => method.AddParameter("IJobExecutionContext", "context")
                                .AddOptionalCancellationTokenParameter(this)
                                .AddStatement("return Task.CompletedTask;"));
                    }

                    // Async() alone turns a ValueTask return type back into Task, so the shape is chosen here.
                    @class.AddMethod("Task", "JobWasExecuted", method => method
                        .Async(asValueTask: usesQuartz4)
                        .AddParameter("IJobExecutionContext", "context")
                        .AddParameter("JobExecutionException" + (OutputTarget.GetProject().NullableEnabled ? "?" : ""), "jobException")
                        .AddOptionalCancellationTokenParameter(this)
                        .AddStatement("await using var scopedServiceProvider = _serviceProvider.CreateAsyncScope();")
                        .AddStatement("var client = scopedServiceProvider.ServiceProvider.GetRequiredService<IClient>();")
                        .AddStatement(new CSharpIfStatement("jobException is not null")
                            .AddStatement("client.Notify(jobException, HandledState.ForUnhandledException());")));
                });
        }

        public override bool CanRunTemplate()
        {
            return TryGetTemplate<ICSharpFileBuilderTemplate>(QuartzConfigurationRole, out _);
        }

        private const string QuartzConfigurationRole = "Distribution.DependencyInjection.Quartz";

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
