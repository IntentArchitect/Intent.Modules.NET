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

namespace Intent.Modules.Aws.Sqs.Templates.SqsMessageDispatcherInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SqsMessageDispatcherInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Sqs.SqsMessageDispatcherInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SqsMessageDispatcherInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Amazon.Lambda.SQSEvents")
                .AddInterface($"ISqsMessageDispatcher", @interface =>
                {
                    @interface.AddMethod("Task", "DispatchAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("IServiceProvider", "scopedServiceProvider");
                        method.AddParameter("SQSEvent.SQSMessage", "sqsMessage");
                        method.AddParameter("CancellationToken", "cancellationToken");
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
