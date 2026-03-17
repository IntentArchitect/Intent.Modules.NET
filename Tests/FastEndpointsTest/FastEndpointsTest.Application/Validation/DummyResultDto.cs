using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.Validation
{
    public record DummyResultDto
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