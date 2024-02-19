using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Services.People
{
    public record TalkToPersonCommand
    {
        public TalkToPersonCommand()
        {
            Message = null!;
            FirstName = null!;
            LastName = null!;
        }

        public string Message { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}