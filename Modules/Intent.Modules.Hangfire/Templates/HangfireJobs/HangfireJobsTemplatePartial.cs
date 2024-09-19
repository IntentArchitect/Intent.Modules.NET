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
                    });
                    @class.AddMethod("Task", "ExecuteAsync", method =>
                    {
                        AddUsing("System");

                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());

                        if (model.GetJobOptions().DisallowConcurrentExecution())
                        {
                            method.AddAttribute("DisableConcurrentExecution", attConfig =>
                            {
                                attConfig.AddArgument(model.GetJobOptions().ConcurrentExecutionTimeout().ToString());
                            });
                        }
                        method.AddStatement($@"throw new NotImplementedException(""Your implementation here..."");");
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