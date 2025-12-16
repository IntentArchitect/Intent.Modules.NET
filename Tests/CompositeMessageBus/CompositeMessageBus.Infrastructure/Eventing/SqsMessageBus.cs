using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using CompositeMessageBus.Application.Common.Eventing;
using CompositeMessageBus.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsMessageBus", Version = "1.0")]

namespace CompositeMessageBus.Infrastructure.Eventing
{
    public class SqsMessageBus : IEventBus
    {
        public const string AddressKey = "address";
        private readonly IAmazonSQS _sqsClient;
        private readonly List<MessageEntry> _messageQueue = [];
        private readonly Dictionary<string, SqsPublisherEntry> _lookup;

        public SqsMessageBus(IOptions<SqsPublisherOptions> options, IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
            _lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, null));
        }

        public void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            throw new NotSupportedException("Publishing with additional data is not supported by this message bus provider.");
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, null));
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

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            if (_messageQueue.Count == 0)
            {
                return;
            }

            var groupedMessages = _messageQueue.GroupBy(entry =>
            {
                var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                return publisherEntry.QueueUrl;
            });

            foreach (var group in groupedMessages)
            {
                foreach (var entry in group)
                {
                    var publisherEntry = _lookup[entry.Message.GetType().FullName!];
                    var sqsMessage = CreateSqsMessage(entry, publisherEntry);
                    await _sqsClient.SendMessageAsync(sqsMessage, cancellationToken);
                }
            }

            _messageQueue.Clear();
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

        private static SendMessageRequest CreateSqsMessage(MessageEntry messageEntry, SqsPublisherEntry publisherEntry)
        {
            var messageBody = JsonSerializer.Serialize(messageEntry.Message);

            var messageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                ["MessageType"] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = messageEntry.Message.GetType().FullName!
                }
            };

            if (messageEntry.AdditionalData != null)
            {
                foreach (var kvp in messageEntry.AdditionalData)
                {
                    messageAttributes[kvp.Key] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = kvp.Value.ToString()
                    };
                }
            }

            var sqsMessage = new SendMessageRequest
            {
                QueueUrl = publisherEntry.QueueUrl,
                MessageBody = messageBody,
                MessageAttributes = messageAttributes
            };
            return sqsMessage;
        }

        private sealed record MessageEntry(object Message, IDictionary<string, object>? AdditionalData);

    }
}