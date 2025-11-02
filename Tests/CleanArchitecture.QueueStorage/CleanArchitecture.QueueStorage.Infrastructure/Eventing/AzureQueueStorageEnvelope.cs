using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.AzureQueueStorageEnvelope", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Infrastructure.Eventing
{
    public class AzureQueueStorageEnvelope
    {
        public AzureQueueStorageEnvelope(object payload)
        {
            MessageType = payload.GetType().FullName;
            Payload = payload;
        }

        public string MessageType { get; set; }
        public object Payload { get; set; }
    }
}