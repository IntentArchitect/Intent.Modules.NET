using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SolaceSystems.Solclient.Messaging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.SolaceConsumer", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class SolaceConsumer
    {
        private IFlow? _flow;
        private readonly ISession _session;
        private readonly DispatchResolver _dispatchResolver;
        private readonly MessageSerializer _messageSerializer;
        private readonly IServiceProvider _serviceProvider;

        public SolaceConsumer(ISession session,
            DispatchResolver dispatchResolver,
            MessageSerializer messageSerializer,
            IServiceProvider serviceProvider)
        {
            _session = session;
            _dispatchResolver = dispatchResolver;
            _messageSerializer = messageSerializer;
            _serviceProvider = serviceProvider;
        }

        public void Start(QueueConfig config)
        {
            using (var queue = ContextFactory.Instance.CreateQueue(config.QueueName))
            {
                // Set queue permissions to "consume" and access-type to "nonexclusive"
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
                    Selector = config.Selector ?? ""
                }, queue, null, HandleMessageEvent, FlowEventHandler);
                _flow.Start();
            }
        }

        private void HandleMessageEvent(object source, MessageEventArgs args)
        {
            Task.Run(() =>
            {
                using (IMessage message = args.Message)
                {

                    var deserializedMessage = _messageSerializer.DeserializeMessage(message.BinaryAttachment);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dispatcher = _dispatchResolver.ResolveDispatcher(deserializedMessage.GetType(), scope.ServiceProvider);
                        var dispatchTask = (Task)dispatcher.GetType()
                            .GetMethod("Dispatch")
                            .Invoke(dispatcher, new object[] { deserializedMessage, default(CancellationToken) });
                        dispatchTask.Wait();
                    }

                    _flow!.Ack(message.ADMessageId);
                }
            });
        }

        private void FlowEventHandler(object? sender, FlowEventArgs e)
        {
        }

        public void Stop()
        {
            _flow?.Stop();
            _flow?.Dispose();
        }
    }

}