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

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventBusInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EventBusInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.EventBusInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventBusInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface("IEventBus", @class => @class
                    .AddMethod("void", "Publish", method => method
                        .AddGenericParameter("T")
                        .AddParameter("T", "message")
                        .AddGenericTypeConstraint("T", constraint => constraint.AddType(this.GetEventInterfaceName()))
                    )
                    .AddMethod("Task", "FlushAllAsync", method => method
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                    )
                );
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