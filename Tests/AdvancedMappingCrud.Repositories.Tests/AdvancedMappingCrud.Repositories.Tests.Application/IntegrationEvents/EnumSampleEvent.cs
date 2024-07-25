using AdvancedMappingCrud.Repositories.Tests.Application.IntegrationEvents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Eventing.Messages
{
    public record EnumSampleEvent
    {
        public EnumSampleEvent()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public SampleType SampleType { get; init; }
    }
}