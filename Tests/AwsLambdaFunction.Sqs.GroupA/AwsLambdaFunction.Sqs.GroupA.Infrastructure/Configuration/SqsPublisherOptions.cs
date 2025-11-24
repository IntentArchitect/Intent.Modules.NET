using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.Sqs.SqsPublisherOptions", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Infrastructure.Configuration
{
    public class SqsPublisherOptions
    {
        private readonly List<SqsPublisherEntry> _entries = [];

        public IReadOnlyList<SqsPublisherEntry> Entries => _entries;

        public void AddQueue<TMessage>(string queueUrl)
        {
            ArgumentNullException.ThrowIfNull(queueUrl);
            _entries.Add(new SqsPublisherEntry(typeof(TMessage), queueUrl));
        }
    }

    public record SqsPublisherEntry(Type MessageType, string QueueUrl);
}