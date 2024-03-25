using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs
{
    public class MockEntityDto
    {
        public MockEntityDto()
        {
        }

        public static MockEntityDto Create()
        {
            return new MockEntityDto
            {
            };
        }
    }
}