using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Eventing.Messages
{
    public record TestClassMappingVOEvent
    {
        public TestClassMappingVOEvent()
        {
            TestVO = null!;
        }

        public TestVODto TestVO { get; init; }
    }
}