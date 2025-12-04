using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureServiceBusMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureServiceBus.AzureServiceBusMessageBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureServiceBusMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation);
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Transactions")
                .AddUsing("Azure.Messaging.ServiceBus")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"AzureServiceBusMessageBus", @class =>
                {
                    @class.ImplementsInterface(this.GetBusInterfaceName());
                    @class.AddField("List<MessageEntry>", "_messageQueue", field => field.PrivateReadOnly().WithAssignment(new CSharpStatement("[]")));
                    @class.AddField("Dictionary<string, string>", "_lookup", field => field.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ServiceBusClient", "serviceBusClient", param => param.IntroduceReadonlyField());
                        ctor.AddParameter($"IOptions<{this.GetAzureServiceBusPublisherOptionsName()}>", "options");

                        ctor.AddStatement("_lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.QueueOrTopicName);");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");

                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, null));");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");

                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, additionalData));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");

                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, null));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var TMessage);
                        method.AddGenericTypeConstraint(TMessage, c => c.AddType("class"));
                        method.AddParameter(TMessage, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");

                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, additionalData));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddIfStatement("_messageQueue.Count == 0", fi => { fi.AddStatement("return;"); });

                        method.AddStatement(
                            "using var scope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);");

                        method.AddForEachStatement("message", "_messageQueue", fe =>
                        {
                            fe.AddStatement("var queueOrTopicName = _lookup[message.Message.GetType().FullName!];");
                            fe.AddStatement("await using var sender = _serviceBusClient.CreateSender(queueOrTopicName);");
                            fe.AddStatement("var serviceBusMessage = CreateServiceBusMessage(message);");
                            fe.AddStatement("await sender.SendMessageAsync(serviceBusMessage, cancellationToken);");
                        });

                        method.AddStatement("scope.Complete();");
                        method.AddStatement("_messageQueue.Clear();");
                    });

                    @class.AddMethod("ServiceBusMessage", "CreateServiceBusMessage", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("MessageEntry", "message");

                        method.AddStatement("var serializedMessage = JsonSerializer.Serialize(message.Message);");
                        method.AddStatement("var serviceBusMessage = new ServiceBusMessage(serializedMessage);");
                        method.AddStatement("""serviceBusMessage.ApplicationProperties["MessageType"] = message.Message.GetType().FullName;""");
                        method.AddIfStatement("message.AdditionalData != null",
                            ifs =>
                            {
                                ifs.AddForEachStatement("dataEnty", "message.AdditionalData",
                                    fe => { fe.AddStatement("serviceBusMessage.ApplicationProperties[dataEnty.Key] = dataEnty.Value;"); });
                            });
                        method.AddStatement("return serviceBusMessage;");
                    });

                    @class.AddNestedRecord("MessageEntry", record =>
                    {
                        record.Private();
                        record.Sealed();
                        record.AddPrimaryConstructor(ctor =>
                        {
                            ctor.AddParameter("object", "Message");
                            ctor.AddParameter("IDictionary<string, object>?", "AdditionalData");
                        });
                    });
                });
        }
        
        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("AzureServiceBus:ConnectionString", "");
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