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

namespace Intent.Modules.Eventing.Solace.Templates.SolaceConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SolaceConsumerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.SolaceConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SolaceConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddUsing("SolaceSystems.Solclient.Messaging")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"SolaceConsumer", @class =>
                {
                    @class.AddField("IFlow?", "_flow", p => p.Private());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ISession", "session", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("DispatchResolver", "dispatchResolver", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("MessageSerializer", "messageSerializer", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IServiceProvider", "serviceProvider", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("void", "Start", method =>
                    {
                        method.AddParameter("QueueConfig", "config");
                        method.AddStatements(@"using (var queue = ContextFactory.Instance.CreateQueue(config.QueueName))
			{
				// Set queue permissions to ""consume"" and access-type to ""nonexclusive""
				EndpointProperties endpointProps = new EndpointProperties()
				{
					Permission = EndpointProperties.EndpointPermission.Consume,
					AccessType = EndpointProperties.EndpointAccessType.NonExclusive,
				};
				// Provision it, and do not fail if it already exists
				_session.Provision(queue, endpointProps, ProvisionFlag.IgnoreErrorIfEndpointAlreadyExists | ProvisionFlag.WaitForConfirm, null);

				foreach (var topicName in config.SubscribedMessages.Where(message => message.TopicName != null).Select(message => message.TopicName))
				{
					var topic = ContextFactory.Instance.CreateTopic(topicName);
					_session.Subscribe(queue, topic, SubscribeFlag.WaitForConfirm, null);
				}

				// Create and start flow to the newly provisioned queue
				// NOTICE HandleMessageEvent as the message event handler 
				// and HandleFlowEvent as the flow event handler
				_flow = _session.CreateFlow(new FlowProperties()
				{
					AckMode = MessageAckMode.ClientAck,
                    WindowSize = Math.Max(1, Math.Min(255, config.MaxFlows ?? 1)),
                    Selector = config.Selector ?? """"
				}, queue, null, HandleMessageEvent, FlowEventHandler);
				_flow.Start();
			}
".ConvertToStatements());
                    });

                    @class.AddMethod("void", "HandleMessageEvent", method =>
                    {
                        method
                            .Private()
                            .AddParameter("object", "source")
                            .AddParameter("MessageEventArgs", "args");
                        method.AddStatements(@"Task.Run(() => {
				using (IMessage message = args.Message)
				{

					var deserializedMessage = _messageSerializer.DeserializeMessage(message.BinaryAttachment);

					using (var scope = _serviceProvider.CreateScope())
					{
						var dispatcher = _dispatchResolver.ResolveDispatcher(deserializedMessage.GetType(), scope.ServiceProvider);
						var dispatchTask = (Task)dispatcher.GetType()
							.GetMethod(""Dispatch"")
							.Invoke(dispatcher, new object[] { deserializedMessage, default(CancellationToken) });
						dispatchTask.Wait();
					}

					_flow!.Ack(message.ADMessageId);
				}
			});".ConvertToStatements());
                    });

                    @class.AddMethod("void", "FlowEventHandler", method =>
                    {
                        method.Private();
                        method.AddParameter("object?", "sender");
                        method.AddParameter("FlowEventArgs", "e");
                    });

                    @class.AddMethod("void", "Stop", method =>
                    {
                        method.AddStatement("_flow?.Stop();");
                        method.AddStatement("_flow?.Dispose();");
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