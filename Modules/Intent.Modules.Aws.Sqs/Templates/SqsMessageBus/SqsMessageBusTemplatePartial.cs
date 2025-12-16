using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Aws.Sqs.Templates.SqsPublisherOptions;
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

namespace Intent.Modules.Aws.Sqs.Templates.SqsMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SqsMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Sqs.SqsMessageBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SqsMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusImplementation);
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
                .AddClass($"SqsMessageBus", @class =>
                {
                    @class.ImplementsInterface(this.GetBusInterfaceName());

                    @class.AddField("IAmazonSQS", "_sqsClient", field => field.PrivateReadOnly());
                    @class.AddField("List<MessageEntry>", "_messageQueue", field => field.PrivateReadOnly().WithAssignment(new CSharpStatement("[]")));
                    @class.AddField("Dictionary<string, SqsPublisherEntry>", "_lookup", field => field.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetTypeName(SqsPublisherOptionsTemplate.TemplateId)}>", "options");
                        ctor.AddParameter("IAmazonSQS", "sqsClient");

                        ctor.AddStatement("_sqsClient = sqsClient;");
                        ctor.AddStatement("_lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);");
                    });

                    @class.AddMethod("void", "Publish", method =>
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

                        method.AddStatement("_messageQueue.Add(new MessageEntry(message, null));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddIfStatement("_messageQueue.Count == 0", fi =>
                        {
                            fi.AddStatement("return;");
                            fi.SeparatedFromNext();
                        });

                        method.AddStatements("""
                            var groupedMessages = _messageQueue.GroupBy(entry =>
                            {
                                var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                                return publisherEntry.QueueUrl;
                            });
                            """.ConvertToStatements());

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


                    @class.AddMethod("SendMessageRequest", "CreateSqsMessage", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("MessageEntry", "messageEntry");
                        method.AddParameter("SqsPublisherEntry", "publisherEntry");

                        method.AddStatement("var messageBody = JsonSerializer.Serialize(messageEntry.Message);");
                        method.AddStatement("""
                            var messageAttributes = new Dictionary<string, MessageAttributeValue>
                            {
                                ["MessageType"] = new MessageAttributeValue
                                {
                                    DataType = "String",
                                    StringValue = messageEntry.Message.GetType().FullName!
                                }
                            };
                            """, stmt => stmt.SeparatedFromPrevious());
                        method.AddIfStatement("messageEntry.AdditionalData != null", ifs =>
                        {
                            ifs.AddForEachStatement("kvp", "messageEntry.AdditionalData", fe =>
                            {
                                fe.AddStatement("""
                                    messageAttributes[kvp.Key] = new MessageAttributeValue
                                    {
                                        DataType = "String",
                                        StringValue = kvp.Value.ToString()
                                    };
                                    """);
                            });
                        });
                        method.AddStatement("""
                            var sqsMessage = new SendMessageRequest
                            {
                                QueueUrl = publisherEntry.QueueUrl,
                                MessageBody = messageBody,
                                MessageAttributes = messageAttributes
                            };
                            """, stmt => stmt.SeparatedFromPrevious());
                        method.AddStatement("return sqsMessage;");
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
