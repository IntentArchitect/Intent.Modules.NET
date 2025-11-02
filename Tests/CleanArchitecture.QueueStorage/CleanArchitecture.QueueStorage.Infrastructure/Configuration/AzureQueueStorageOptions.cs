using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageOptions", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Configuration
{
    public class AzureQueueStorageOptions
    {
        public string? DefaultEndpoint { get; set; }
        public Dictionary<string, QueueDefinition> Queues { get; set; } = new Dictionary<string, QueueDefinition>();
        public Dictionary<string, string> QueueTypeMap { get; set; } = new Dictionary<string, string>();
    }

    public class QueueDefinition
    {
        public string? QueueName { get; set; }
        public string? Endpoint { get; set; }
        public bool CreateQueue { get; set; }
        public int MaxMessages { get; set; } = 10;
    }
}