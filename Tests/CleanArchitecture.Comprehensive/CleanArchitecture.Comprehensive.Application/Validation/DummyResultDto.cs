using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Validation
{
    public class DummyResultDto
    {
        public DummyResultDto()
        {
        }

        public static DummyResultDto Create()
        {
            return new DummyResultDto
            {
            };
        }
    }
}