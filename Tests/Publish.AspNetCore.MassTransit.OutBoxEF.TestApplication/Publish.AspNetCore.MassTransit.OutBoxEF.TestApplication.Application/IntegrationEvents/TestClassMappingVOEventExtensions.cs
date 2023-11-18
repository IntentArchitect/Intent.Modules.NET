using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Eventing.Messages
{
    public static class TestClassMappingVOEventExtensions
    {
        public static TestClassMappingVOEvent MapToTestClassMappingVOEvent(this ClassWithVO projectFrom)
        {
            return new TestClassMappingVOEvent
            {
                TestVO = TestVODtoExtensions.MapToTestVODto(projectFrom.TestVO),
            };
        }
    }
}