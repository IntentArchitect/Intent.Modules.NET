using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace N_ServiceBus.AzureServiceBus.Eventing.Messages
{
    public record OrderAnimal
    {
        public OrderAnimal()
        {
            Name = null!;
        }

        public string Name { get; init; }
    }
}