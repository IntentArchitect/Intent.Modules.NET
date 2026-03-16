using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation
{
    public record SelfRefWithCompDto
    {
        public SelfRefWithCompDto()
        {
            Entry = null!;
            SelfRefDtos = null!;
            Composite = null!;
        }

        public string Entry { get; init; }
        public List<SelfRefWithCompDto> SelfRefDtos { get; init; }
        public SelfRefCompositeDto Composite { get; init; }
    }
}