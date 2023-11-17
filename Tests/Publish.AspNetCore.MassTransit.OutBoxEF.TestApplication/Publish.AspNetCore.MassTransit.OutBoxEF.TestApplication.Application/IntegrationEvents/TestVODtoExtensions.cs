using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Mapping;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.DtoExtensions", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Eventing.Messages
{
    public static class TestVODtoExtensions
    {
        public static TestVODto MapToTestVODto(this TestVO projectFrom)
        {
            return new TestVODto
            {
                Name = projectFrom.Name,
            };
        }
    }
}