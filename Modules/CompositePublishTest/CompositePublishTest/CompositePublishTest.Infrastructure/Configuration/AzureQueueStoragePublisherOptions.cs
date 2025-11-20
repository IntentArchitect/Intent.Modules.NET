using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CompositePublishTest.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration options for Azure Queue Storage message publishing.
    /// Maps message types to their destination queues with optional custom endpoints.
    /// </summary>
    public class AzureQueueStorageOptions
    {
        public string? DefaultEndpoint { get; set; }
        public Dictionary<string, QueueDefinition> Queues { get; set; } = new();
        public Dictionary<string, string> QueueTypeMap { get; set; } = new();
    }
    
    public class QueueDefinition
    {
        public string? QueueName { get; set; }
        public string? Endpoint { get; set; }
        public bool CreateQueue { get; set; }
        public int MaxMessages { get; set; } = 10;
    }
}
