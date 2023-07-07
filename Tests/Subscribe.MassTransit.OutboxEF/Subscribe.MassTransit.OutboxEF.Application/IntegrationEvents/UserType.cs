using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventEnum", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Application.IntegrationEvents
{
    public enum UserType
    {
        Normal,
        Admin
    }
}