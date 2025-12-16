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
                    .WithComments([
                        "/// <summary>",
                        "/// Provides an abstraction for dispatching messages to one or more underlying message brokers.",
                        "/// Messages are queued via <see cref=\"Publish{TMessage}\"/> (fan-out / broadcast semantics) or <see cref=\"Send{TMessage}\"/> (point-to-point semantics) and are flushed in batches when <see cref=\"FlushAllAsync\"/> is invoked.",
                        "/// </summary>"
                    ])
                    .AddMethod("void", "Publish", m => m
                        .AddGenericParameter("TMessage")
                        .AddParameter("TMessage", "message")
                        .AddGenericTypeConstraint("TMessage", c => c.AddType("class"))
                        .WithComments([
                            "/// <summary>",
                            "/// Queues a message to be published to all interested subscribers (topic / fan-out semantics).",
                            "/// </summary>",
                            "/// <typeparam name=\"TMessage\">The concrete message type.</typeparam>",
                            "/// <param name=\"message\">The message instance to publish.</param>",
                            "/// <remarks>",
                            "/// The message is buffered until <see cref=\"FlushAllAsync\"/> is called.",
                            "/// </remarks>"
                        ])
                    )
                    .AddMethod("void", "Send", m => m
                        .AddGenericParameter("TMessage")
                        .AddParameter("TMessage", "message")
                        .AddGenericTypeConstraint("TMessage", c => c.AddType("class"))
                        .WithComments([
                            "/// <summary>",
                            "/// Queues a message for point-to-point delivery to a single consumer endpoint.",
                            "/// </summary>",
                            "/// <typeparam name=\"TMessage\">The concrete message type.</typeparam>",
                            "/// <param name=\"message\">The message instance to send.</param>",
                            "/// <remarks>",
                            "/// The message is buffered until <see cref=\"FlushAllAsync\"/> is called. Use send for commands or direct messages; use publish for events.",
                            "/// </remarks>"
                        ])
                    )
                    .AddMethod("Task", "FlushAllAsync", m => m
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .WithComments([
                            "/// <summary>",
                            "/// Flushes and dispatches all queued messages to the underlying broker(s).",
                            "/// </summary>",
                            "/// <param name=\"cancellationToken\">Cancellation token for the asynchronous operation.</param>",
                            "/// <remarks>",
                            "/// Implementations may optimize by batching publish/send operations. Internal buffers are typically cleared after a successful flush.",
                            "/// </remarks>"
                        ])
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
