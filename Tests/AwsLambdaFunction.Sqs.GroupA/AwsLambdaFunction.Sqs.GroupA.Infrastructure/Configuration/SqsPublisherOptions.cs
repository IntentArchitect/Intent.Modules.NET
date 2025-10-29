using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsPublisherOptions", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Infrastructure.Configuration
{
    public class SqsPublisherOptions
    {
        private readonly List<PublisherEntry> _entries = [];

        public IReadOnlyList<PublisherEntry> Entries => _entries;

        public void AddQueue<TMessage>(string queueUrl)
        {
            ArgumentNullException.ThrowIfNull(queueUrl);
            _entries.Add(new PublisherEntry(typeof(TMessage), queueUrl));
        }
    }

    public record PublisherEntry(Type MessageType, string QueueUrl);
}