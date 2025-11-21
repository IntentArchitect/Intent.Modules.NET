using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Aws.Sqs.Templates.SqsPublisherOptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Sqs.Templates.SqsEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SqsEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Sqs.SqsEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SqsEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AwsSdkSqs(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Amazon.SQS")
                .AddUsing("Amazon.SQS.Model")
                .AddUsing("Microsoft.Extensions.Options")
                .AddClass($"SqsEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetMessageBusInterfaceName());
                    @class.AddField("IAmazonSQS", "_sqsClient", field => field.PrivateReadOnly());
                    @class.AddField("List<MessageEntry>", "_messageQueue", field => field.PrivateReadOnly().WithAssignment(new CSharpStatement("[]")));
                    @class.AddField("Dictionary<string, PublisherEntry>", "_lookup", field => field.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetTypeName(SqsPublisherOptionsTemplate.TemplateId)}>", "options");
                        ctor.AddParameter("IAmazonSQS", "sqsClient");

                        ctor.AddStatement("_sqsClient = sqsClient;");
                        ctor.AddStatement("_lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T);
                        method.AddGenericTypeConstraint(T, c => c.AddType("class"));
                        method.AddParameter(T, "message");

                        method.AddStatement("ValidateMessage(message);");
                        method.AddStatement("_messageQueue.Add(new MessageEntry(message));");
                    });
                    
                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T);
                        method.AddGenericTypeConstraint(T, c => c.AddType("class"));
                        method.AddParameter(T, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");

                        method.AddStatement("// Note: AWS SQS does not support additional data in this implementation, ignoring parameter");
                        method.AddStatement("Publish(message);");
                    });
                    
                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("T", out var T);
                        method.AddGenericTypeConstraint(T, c => c.AddType("class"));
                        method.AddParameter(T, "message");

                        method.AddStatement("Publish(message);");
                    });
                    
                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("T", out var T);
                        method.AddGenericTypeConstraint(T, c => c.AddType("class"));
                        method.AddParameter(T, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");

                        method.AddStatement("Publish(message, additionalData);");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddIfStatement("_messageQueue.Count == 0", fi =>
                        {
                            fi.AddStatement("return;");
                        });

                        method.AddStatement("""
                            var groupedMessages = _messageQueue.GroupBy(entry =>
                            {
                                var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                                return publisherEntry.QueueUrl;
                            });
                            """, stmt => stmt.SeparatedFromPrevious());

                        method.AddForEachStatement("group", "groupedMessages", fe =>
                        {
                            fe.AddForEachStatement("entry", "group", innerFe =>
                            {
                                innerFe.AddStatement("var publisherEntry = _lookup[entry.Message.GetType().FullName!];");
                                innerFe.AddStatement("var sqsMessage = CreateSqsMessage(entry, publisherEntry);");
                                innerFe.AddStatement("await _sqsClient.SendMessageAsync(sqsMessage, cancellationToken);");
                            });
                        });

                        method.AddStatement("_messageQueue.Clear();", stmt => stmt.SeparatedFromPrevious());
                    });

                    @class.AddMethod("void", "ValidateMessage", method =>
                    {
                        method.Private();
                        method.AddParameter("object", "message");
                        method.AddIfStatement("!_lookup.TryGetValue(message.GetType().FullName!, out _)", ifs =>
                        {
                            ifs.AddStatement("""throw new Exception($"The message type '{message.GetType().FullName}' is not registered.");""");
                        });
                    });

                    @class.AddMethod("SendMessageRequest", "CreateSqsMessage", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("MessageEntry", "messageEntry");
                        method.AddParameter("PublisherEntry", "publisherEntry");

                        method.AddStatement("var messageBody = JsonSerializer.Serialize(messageEntry.Message);");
                        method.AddStatement("""
                            var sqsMessage = new SendMessageRequest
                            {
                                QueueUrl = publisherEntry.QueueUrl,
                                MessageBody = messageBody,
                                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                                {
                                    ["MessageType"] = new MessageAttributeValue
                                    {
                                        DataType = "String",
                                        StringValue = messageEntry.Message.GetType().FullName!
                                    }
                                }
                            };
                            """, stmt => stmt.SeparatedFromPrevious());
                        method.AddStatement("return sqsMessage;");
                    });
                });

            CSharpFile.AfterBuild(file =>
            {
                file.AddClass("MessageEntry", @class =>
                {
                    @class.Internal();
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("object", "message", param => param.IntroduceProperty());
                    });
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
