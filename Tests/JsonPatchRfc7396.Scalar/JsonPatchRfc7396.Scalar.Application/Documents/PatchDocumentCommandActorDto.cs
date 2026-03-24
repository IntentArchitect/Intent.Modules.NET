using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record PatchDocumentCommandActorDto
    {
        public PatchDocumentCommandActorDto()
        {
            UserId = null!;
            DisplayName = null!;
        }

        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }
}