using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record DocumentDocumentChangeActorDto
    {
        public DocumentDocumentChangeActorDto()
        {
            UserId = null!;
            DisplayName = null!;
        }

        public string UserId { get; init; }
        public string DisplayName { get; init; }
    }
}