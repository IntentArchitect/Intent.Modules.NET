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

namespace Intent.Modules.Eventing.Solace.Templates.SolaceEventDispatcherInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SolaceEventDispatcherInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.SolaceEventDispatcherInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SolaceEventDispatcherInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"ISolaceEventDispatcher", @interface =>
                {
                    @interface.AddGenericParameter("TMessage", out var tMessage).AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                    @interface.AddMethod("Task", "Dispatch", method =>
                    {
                        method.AddParameter(tMessage, "message");
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