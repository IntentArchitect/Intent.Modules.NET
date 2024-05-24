using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class TestDCDto
    {
        public TestDCDto()
        {
        }

        public Guid Id { get; set; }
        public int Index { get; set; }

        public static TestDCDto Create(Guid id, int index)
        {
            return new TestDCDto
            {
                Id = id,
                Index = index
            };
        }
    }
}