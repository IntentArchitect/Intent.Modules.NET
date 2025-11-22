using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MassTransitEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.MassTransitEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MassTransitEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MassTransit")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("System.Linq")
                .AddClass("MassTransitEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetBusInterfaceName());

                    var listFieldDefault = outputTarget.GetProject().GetLanguageVersion().Major < 12
                        ? new CSharpStatement("new List<MessageEntry>()")
                        : new CSharpStatement("[]");
                    @class.AddField("List<MessageEntry>", "_messagesToDispatch", field => field
                        .PrivateReadOnly()
                        .WithAssignment(listFieldDefault));

                    @class.AddConstructor(ctor => { ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField()); });

                    @class.AddProperty("ConsumeContext?", "ConsumeContext");

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddStatement("_messagesToDispatch.Add(new MessageEntry(message, null, DispatchType.Publish));");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddParameter("IDictionary<string, object>", "additionalData")
                            .AddStatement("_messagesToDispatch.Add(new MessageEntry(message, additionalData, DispatchType.Publish));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddStatement("_messagesToDispatch.Add(new MessageEntry(message, null, DispatchType.Send));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddParameter("IDictionary<string, object>", "additionalData")
                            .AddStatement("_messagesToDispatch.Add(new MessageEntry(message, additionalData, DispatchType.Send));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddParameter("Uri", "address")
                            .AddStatement("var additionalData = new Dictionary<string, object> { { \"address\", address.ToString() } };")
                            .AddStatement("_messagesToDispatch.Add(new MessageEntry(message, additionalData, DispatchType.Send));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement("var messagesToPublish = _messagesToDispatch.Where(x => x.DispatchType == DispatchType.Publish).ToList();");
                        method.AddStatement("var messagesToSend = _messagesToDispatch.Where(x => x.DispatchType == DispatchType.Send).ToList();");

                        method.AddStatement("await PublishMessagesAsync(messagesToPublish, cancellationToken);");
                        method.AddStatement("await SendMessagesAsync(messagesToSend, cancellationToken);");

                        method.AddStatement("_messagesToDispatch.Clear();");
                    });

                    @class.AddMethod("Task", "PublishMessagesAsync", method =>
                    {
                        method.Private().Async();
                        method.AddParameter("List<MessageEntry>", "messagesToPublish");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddIfStatement("ConsumeContext is not null", block =>
                        {
                            block.AddStatement("await ConsumeContext!.PublishBatch(messagesToPublish.Select(x => x.Message), cancellationToken).ConfigureAwait(false);");
                            block.AddStatement("return;");
                        });

                        method.AddStatement("var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();");
                        method.AddStatement("await publishEndpoint.PublishBatch(messagesToPublish.Select(x => x.Message), cancellationToken).ConfigureAwait(false);");
                    });

                    @class.AddMethod("Task", "SendMessagesAsync", method =>
                    {
                        method.Private().Async();
                        method.AddParameter("List<MessageEntry>", "messagesToSend");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddForEachStatement("toSend", "messagesToSend", fe =>
                        {
                            fe.AddStatement("Uri? address = null;");
                            fe.AddIfStatement("toSend.AdditionalData?.TryGetValue(\"address\", out var addressObj) == true", block =>
                            {
                                block.AddStatement("address = new Uri((string)addressObj);");
                            });

                            fe.AddIfStatement("ConsumeContext is not null", block =>
                            {
                                block.AddIfStatement("address is null", b => b.AddStatement("await ConsumeContext!.Send(toSend.Message, cancellationToken).ConfigureAwait(false);"));
                                block.AddElseStatement(b =>
                                {
                                    b.AddStatement("var endpoint = await ConsumeContext!.GetSendEndpoint(address).ConfigureAwait(false);");
                                    b.AddStatement("await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);");
                                });
                            });
                            fe.AddElseStatement(block =>
                            {
                                block.AddIfStatement("address is null", b =>
                                {
                                    b.AddStatement("var bus = _serviceProvider.GetRequiredService<IBus>();");
                                    b.AddStatement("await bus.Send(toSend.Message, cancellationToken).ConfigureAwait(false);");
                                });
                                block.AddElseStatement(b =>
                                {
                                    b.AddStatement("var sendEndpointProvider = _serviceProvider.GetRequiredService<ISendEndpointProvider>();");
                                    b.AddStatement("var endpoint = await sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);");
                                    b.AddStatement("await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);");
                                });
                            });
                        });
                    });

                    @class.AddNestedEnum("DispatchType", enumType =>
                    {
                        enumType.Private();
                        enumType.AddLiteral("Publish");
                        enumType.AddLiteral("Send");
                    });

                    @class.AddNestedRecord("MessageEntry", rec =>
                    {
                        rec.Private();
                        rec.AddPrimaryConstructor(ctor =>
                        {
                            ctor.AddParameter("object", "Message");
                            ctor.AddParameter("IDictionary<string, object>?", "AdditionalData");
                            ctor.AddParameter("DispatchType", "DispatchType");
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