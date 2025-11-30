using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventInterface;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.DaprMessageBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.DaprClient(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("Dapr.Client")
                .AddClass("DaprEventBus", @class => @class
                    .ImplementsInterface(this.GetBusInterfaceName())
                    .AddField($"ConcurrentQueue<MessageEntry>", "_messages", f => f.PrivateReadOnly())
                    .AddConstructor(constructor => constructor
                        .AddParameter("DaprClient", "dapr", p => p.IntroduceReadonlyField())
                        .AddStatement($"_messages = new ConcurrentQueue<MessageEntry>();")
                    )
                    .AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddStatement("_messages.Enqueue(new MessageEntry((IEvent)message, null));");
                    }).AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter("IDictionary<string, object>", "additionalData")
                            .AddStatement("_messages.Enqueue(new MessageEntry((IEvent)message, additionalData.ToDictionary(key => key.Key, value => value.ToString())));");
                    }).AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddStatement("_messages.Enqueue(new MessageEntry((IEvent)message, null));");
                    })
                    .AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage)
                            .AddGenericTypeConstraint(TMessage, c => c.AddType("class"))
                            .AddParameter(TMessage, "message")
                            .AddParameter("IDictionary<string, object>", "additionalData")
                            .AddStatement("_messages.Enqueue(new MessageEntry((IEvent)message, additionalData.ToDictionary(key => key.Key, value => value.ToString())));");
                    })
                    .AddMethod("Task", "FlushAllAsync", method => method
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatementBlock("while (_messages.TryDequeue(out var message))", block => block
                                .AddStatement(
                                    """
                                    // We need to make sure that we pass the concrete type to PublishEventAsync,
                                    // which can be accomplished by casting the event to dynamic. This ensures
                                    // that all event fields are properly serialized.
                                    """)
                                .AddIfStatement("message.AdditionalData != null", ifStmt => ifStmt
                                    .AddInvocationStatement("await _dapr.PublishEventAsync", inv => inv
                                        .AddArgument("message.Message.PubsubName")
                                        .AddArgument("message.Message.TopicName")
                                        .AddArgument("(object)message.Message")
                                        .AddArgument("message.AdditionalData")
                                        .AddArgument("cancellationToken")))
                                .AddElseStatement(@else => @else
                                    .AddInvocationStatement("await _dapr.PublishEventAsync", inv => inv
                                        .AddArgument("message.Message.PubsubName")
                                        .AddArgument("message.Message.TopicName")
                                        .AddArgument("(object)message.Message")
                                        .AddArgument("cancellationToken"))
                                )
                        )
                    )
                    .AddNestedRecord("MessageEntry", rec =>
                    {
                        rec.Private();
                        rec.AddPrimaryConstructor(ctor =>
                        {
                            ctor.AddParameter(this.GetEventInterfaceName(), "Message");
                            ctor.AddParameter("Dictionary<string, string>?", "AdditionalData");
                        });
                    })
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