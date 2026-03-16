using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.SelfReferenceValidation
{
    public record UploadWrapperDto
    {
        public UploadWrapperDto()
        {
            Entry = null!;
            SelfRefDtos = null!;
        }

        public string Entry { get; init; }
        public List<SelfRefDto> SelfRefDtos { get; init; }
    }
}