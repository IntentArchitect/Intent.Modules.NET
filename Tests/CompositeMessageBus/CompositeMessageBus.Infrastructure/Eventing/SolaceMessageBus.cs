using CompositeMessageBus.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using SolaceSystems.Solclient.Messaging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Solace.SolaceMessageBus", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class SolaceMessageBus : IEventBus
    {
        public const string AddressKey = "address";
        private readonly List<object> _messagesToPublish = new List<object>();
        private readonly List<object> _messagesToSend = new List<object>();
        private readonly int? _defaultPriority;
        private readonly ISession _session;
        private readonly MessageRegistry _messageRegistry;
        private readonly MessageSerializer _messageSerializer;

        public SolaceMessageBus(ISession session,
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

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Publishing with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messagesToSend.Add(message);
        }

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Sending with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message, Uri address)
            where TMessage : class
        {
            throw new NotSupportedException("Explicit address-based sending is not supported by this message bus provider.");
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

        public void SchedulePublish<TMessage>(TMessage message, DateTime scheduled)
            where TMessage : class
        {
            throw new NotSupportedException("Scheduled publishing is not supported by this message bus provider.");
        }

        public void SchedulePublish<TMessage>(TMessage message, TimeSpan delay)
            where TMessage : class
        {
            throw new NotSupportedException("Scheduled publishing is not supported by this message bus provider.");
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