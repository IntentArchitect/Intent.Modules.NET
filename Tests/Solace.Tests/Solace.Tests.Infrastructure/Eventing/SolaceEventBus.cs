using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application.Common.Eventing;
using SolaceSystems.Solclient.Messaging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.SolaceEventBus", Version = "1.0")]

namespace Solace.Tests.Infrastructure.Eventing
{
    public class SolaceEventBus : IEventBus
    {
        private readonly List<object> _messagesToPublish = new List<object>();
        private readonly List<object> _messagesToSend = new List<object>();
        private readonly ISession _session;
        private readonly MessageRegistry _messageRegistry;
        private readonly MessageSerializer _messageSerializer;

        public SolaceEventBus(ISession session, MessageRegistry messageRegistry, MessageSerializer messageSerializer)
        {
            _session = session;
            _messageRegistry = messageRegistry;
            _messageSerializer = messageSerializer;
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _messagesToPublish.Add(message);
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messagesToSend.Add(message);
        }

        public Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            foreach (var toPublish in _messagesToPublish)
            {
                PublishEvent(_session, toPublish);
            }

            _messagesToPublish.Clear();
            foreach (var toSend in _messagesToSend)
            {
                SendCommand(_session, toSend);
            }
            _messagesToSend.Clear();
            return Task.CompletedTask;
        }

        private void PublishEvent(ISession session, object toPublish)
        {
            using (var message = ContextFactory.Instance.CreateMessage())
            {
                message.Destination = ContextFactory.Instance.CreateTopic(GetDestinationAddress(toPublish));
                message.BinaryAttachment = _messageSerializer.SerializeMessage(toPublish);

                var returnCode = session.Send(message);
                if (returnCode != ReturnCode.SOLCLIENT_OK)
                {
                    throw new Exception($"Unable to publish message so Solace ({toPublish.GetType().Name}) : {returnCode}");
                }
            }
        }

        private void SendCommand(ISession session, object toSend)
        {
            using (var message = ContextFactory.Instance.CreateMessage())
            {
                message.Destination = ContextFactory.Instance.CreateQueue(GetDestinationAddress(toSend));
                message.BinaryAttachment = _messageSerializer.SerializeMessage(toSend);

                var returnCode = session.Send(message);
                if (returnCode != ReturnCode.SOLCLIENT_OK)
                {
                    throw new Exception($"Unable to send command so Solace ({toSend.GetType().Name}) : {returnCode}");
                }
            }
        }

        private string GetDestinationAddress(object message)
        {
            return _messageRegistry.GetConfig(message.GetType()).PublishedDestination;
        }
    }
}