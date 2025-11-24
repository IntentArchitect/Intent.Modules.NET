using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using AwsLambdaFunction.Sqs.GroupA.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupA.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsEventBus", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Infrastructure.Eventing
{
    public class SqsEventBus : IEventBus
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly List<MessageEntry> _messageQueue = [];
        private readonly Dictionary<string, SqsPublisherEntry> _lookup;

        public SqsEventBus(IOptions<SqsPublisherOptions> options, IAmazonSQS sqsClient)
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
            _messageQueue.Add(new MessageEntry(message, additionalData));
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, null));
        }

        public void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
            where TMessage : class
        {
            _messageQueue.Add(new MessageEntry(message, additionalData));
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

        private record MessageEntry(object Message, IDictionary<string, object>? AdditionalData);
    }
}