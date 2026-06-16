using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace N_ServiceBus.AzureServiceBus.Eventing.Messages
{
    public record MakeSoundCommand
    {
    }
}