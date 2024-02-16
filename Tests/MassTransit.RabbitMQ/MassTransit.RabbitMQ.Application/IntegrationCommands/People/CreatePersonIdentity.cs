using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.People
{
    public record CreatePersonIdentity
    {
        public CreatePersonIdentity()
        {
            FirstName = null!;
            LastName = null!;
        }

        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}