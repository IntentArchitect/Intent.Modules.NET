using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class TestEntityDto
    {
        public TestEntityDto()
        {
        }

        public Guid Id { get; set; }

        public static TestEntityDto Create(Guid id)
        {
            return new TestEntityDto
            {
                Id = id
            };
        }
    }
}