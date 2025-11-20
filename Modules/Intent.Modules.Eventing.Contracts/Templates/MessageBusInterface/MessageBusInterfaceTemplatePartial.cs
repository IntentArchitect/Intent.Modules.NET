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

namespace Intent.Modules.Eventing.Contracts.Templates.MessageBusInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageBusInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.MessageBusInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageBusInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface("IMessageBus", @interface => @interface
                    .AddMethod("void", "Publish", m => m
                        .AddGenericParameter("TMessage")
                        .AddParameter("TMessage", "message")
                        .AddGenericTypeConstraint("TMessage", c => c.AddType("class"))
                    )
                    .AddMethod("void", "Publish", m => m
                        .AddGenericParameter("TMessage")
                        .AddParameter("TMessage", "message")
                        .AddParameter("IDictionary<string, object>", "additionalData")
                        .AddGenericTypeConstraint("TMessage", c => c.AddType("class"))
                    )
                    .AddMethod("void", "Send", m => m
                        .AddGenericParameter("TMessage")
                        .AddParameter("TMessage", "message")
                        .AddGenericTypeConstraint("TMessage", c => c.AddType("class"))
                    )
                    .AddMethod("void", "Send", m => m
                        .AddGenericParameter("TMessage")
                        .AddParameter("TMessage", "message")
                        .AddParameter("IDictionary<string, object>", "additionalData")
                        .AddGenericTypeConstraint("TMessage", c => c.AddType("class"))
                    )
                    .AddMethod("Task", "FlushAllAsync", m => m
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
