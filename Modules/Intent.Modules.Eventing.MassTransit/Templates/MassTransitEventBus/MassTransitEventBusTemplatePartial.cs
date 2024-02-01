using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
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
                .AddClass("MassTransitEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetEventBusInterfaceName());
                    @class.AddNestedClass("MessageToSend", nested =>
                    {
                        nested
                            .Private()
                            .AddConstructor(ctor =>
                            {
                                ctor.AddParameter("object", "message", p => p.IntroduceProperty(prop => prop.WithoutSetter()));
                                ctor.AddParameter("Uri?", "address", p => p.IntroduceProperty(prop => prop.WithoutSetter()));
                            });
                    });

                    @class.AddField("List<object>", "_messagesToPublish", field => field
                        .PrivateReadOnly()
                        .WithAssignment(new CSharpStatement("new List<object>()")));
                    @class.AddField("List<MessageToSend>", "_messagesToSend", field => field
                        .PrivateReadOnly()
                        .WithAssignment(new CSharpStatement("new List<MessageToSend>()")));

                    @class.AddConstructor(ctor => { ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField()); });

                    @class.AddProperty("ConsumeContext?", "ConsumeContext");

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddStatement("_messagesToPublish.Add(message);");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddStatement("_messagesToSend.Add(new MessageToSend(message, null));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddParameter("Uri", "address")
                            .AddStatement("_messagesToSend.Add(new MessageToSend(message, address));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddForEachStatement("toSend", "_messagesToSend", fe =>
                        {
                            fe.AddIfStatement("ConsumeContext is not null", block => { block.AddStatement("await SendWithConsumeContext(toSend, cancellationToken);"); });
                            fe.AddElseStatement(block => { block.AddStatement("await SendWithNormalContext(toSend, cancellationToken);"); });
                        });
                        method.AddStatement("_messagesToSend.Clear();", s => s.SeparatedFromPrevious());

                        method.AddIfStatement("ConsumeContext is not null", block => { block.AddStatement("await PublishWithConsumeContext(cancellationToken);"); });
                        method.AddElseStatement(block => { block.AddStatement("await PublishWithNormalContext(cancellationToken);"); });
                        method.AddStatement("_messagesToPublish.Clear();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("Task", "SendWithConsumeContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("MessageToSend", "toSend");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddIfStatement("toSend.Address is null", block => { block.AddStatement("await ConsumeContext!.Send(toSend.Message, cancellationToken).ConfigureAwait(false);"); });
                        method.AddElseStatement(block =>
                        {
                            block.AddStatement("var endpoint = await ConsumeContext!.GetSendEndpoint(toSend.Address).ConfigureAwait(false);");
                            block.AddStatement("await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);");
                        });
                    });

                    @class.AddMethod("Task", "SendWithNormalContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("MessageToSend", "toSend");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddIfStatement("toSend.Address is null", block =>
                        {
                            block.AddStatement("var bus = _serviceProvider.GetRequiredService<IBus>();");
                            block.AddStatement("await bus.Send(toSend.Message, cancellationToken).ConfigureAwait(false);");
                        });
                        method.AddElseStatement(block =>
                        {
                            block.AddStatement("var sendEndpointProvider = _serviceProvider.GetRequiredService<ISendEndpointProvider>();");
                            block.AddStatement("var endpoint = await sendEndpointProvider.GetSendEndpoint(toSend.Address).ConfigureAwait(false);");
                            block.AddStatement("await endpoint.Send(toSend.Message, cancellationToken).ConfigureAwait(false);");
                        });
                    });


                    @class.AddMethod("Task", "PublishWithConsumeContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("await ConsumeContext!.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);");
                    });

                    @class.AddMethod("Task", "PublishWithNormalContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();");

                        method.AddStatement("await publishEndpoint.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);",
                            s => s.SeparatedFromPrevious());
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