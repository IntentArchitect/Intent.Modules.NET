using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using AwsLambdaFunction.Sqs.GroupB.Application.Common.Eventing;
using AwsLambdaFunction.Sqs.GroupB.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsEventBus", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Infrastructure.Eventing
{
    public class SqsEventBus : IEventBus
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly List<MessageEntry> _messageQueue = [];
        private readonly Dictionary<string, PublisherEntry> _lookup;

        public SqsEventBus(IOptions<SqsPublisherOptions> options, IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
            _lookup = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!);
        }

        public void Publish<T>(T message)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(new MessageEntry(message));
        }

        public void Send<T>(T message)
            where T : class
        {
            ValidateMessage(message);
            _messageQueue.Add(new MessageEntry(message));
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

        private void ValidateMessage(object message)
        {
            if (!_lookup.TryGetValue(message.GetType().FullName!, out _))
            {
                throw new Exception($"The message type '{message.GetType().FullName}' is not registered.");
            }
        }

        private static SendMessageRequest CreateSqsMessage(MessageEntry messageEntry, PublisherEntry publisherEntry)
        {
            var messageBody = JsonSerializer.Serialize(messageEntry.Message);

            var sqsMessage = new SendMessageRequest
            {
                QueueUrl = publisherEntry.QueueUrl,
                MessageBody = messageBody,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    ["MessageType"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = messageEntry.Message.GetType().FullName!
                    }
                }
            };
            return sqsMessage;
        }
    }

    internal class MessageEntry
    {
        public MessageEntry(object message)
        {
            Message = message;
        }

        public object Message { get; set; }
    }
}