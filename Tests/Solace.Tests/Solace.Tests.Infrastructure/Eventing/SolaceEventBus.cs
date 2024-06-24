using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
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
        private readonly int? _defaultPriority;
        private readonly ISession _session;
        private readonly MessageRegistry _messageRegistry;
        private readonly MessageSerializer _messageSerializer;

        public SolaceEventBus(ISession session,
            MessageRegistry messageRegistry,
            MessageSerializer messageSerializer,
            IConfiguration configuration)
        {
            _session = session;
            _messageRegistry = messageRegistry;
            _messageSerializer = messageSerializer;
            _defaultPriority = configuration.GetSection("Solace:DefaultSendPriority").Get<int?>();
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
                var messageConfig = _messageRegistry.PublishedMessages[toPublish.GetType()];

                message.Destination = ContextFactory.Instance.CreateTopic(messageConfig.PublishTo);
                message.BinaryAttachment = _messageSerializer.SerializeMessage(toPublish);
                message.Priority = GetPriority(messageConfig.Priority);

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
                var messageConfig = _messageRegistry.PublishedMessages[toSend.GetType()];

                message.Destination = ContextFactory.Instance.CreateQueue(messageConfig.PublishTo);
                message.BinaryAttachment = _messageSerializer.SerializeMessage(toSend);
                message.Priority = GetPriority(messageConfig.Priority);

                var returnCode = session.Send(message);
                if (returnCode != ReturnCode.SOLCLIENT_OK)
                {
                    throw new Exception($"Unable to send command so Solace ({toSend.GetType().Name}) : {returnCode}");
                }
            }
        }

        private int? GetPriority(int? messagePriority)
        {
            int? result = null;
            if (messagePriority != null)
            {
                result = messagePriority.Value;
            }
            else
            {
                result = _defaultPriority;
            }
            if (result != null)
            {
                result = Math.Clamp(result.Value, 0, 255);
            }
            return result;
        }
    }
}