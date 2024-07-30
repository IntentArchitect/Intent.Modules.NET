using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventEnum", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.IntegrationEvents
{
    public enum SampleType
    {
        TypeA,
        TypeB
    }
}