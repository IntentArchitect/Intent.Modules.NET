using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Eventing.Messages
{
    public record TalkToPersonCommand
    {
    }
}