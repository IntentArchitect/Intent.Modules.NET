using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.Animals
{
    public record OrderAnimal
    {
        public OrderAnimal()
        {
            Name = null!;
            Type = null!;
        }

        public string Name { get; init; }
        public string Type { get; init; }
    }
}