using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Publish.Eventing.Messages
{
    public record TestCommand
    {
        public TestCommand()
        {
            Message = null!;
        }

        public string Message { get; init; }
    }
}