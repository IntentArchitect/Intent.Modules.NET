using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class TestVODto
    {
        public TestVODto()
        {
        }

        public Guid Id { get; set; }

        public static TestVODto Create(Guid id)
        {
            return new TestVODto
            {
                Id = id
            };
        }
    }
}