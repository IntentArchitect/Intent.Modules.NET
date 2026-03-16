using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation
{
    public record SelfRefDto
    {
        public SelfRefDto()
        {
            Entry = null!;
            SelfRefDtos = null!;
        }

        public string Entry { get; init; }
        public List<SelfRefDto> SelfRefDtos { get; init; }
    }
}