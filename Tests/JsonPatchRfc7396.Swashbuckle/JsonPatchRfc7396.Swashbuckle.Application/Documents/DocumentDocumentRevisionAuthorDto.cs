using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record DocumentDocumentRevisionAuthorDto
    {
        public DocumentDocumentRevisionAuthorDto()
        {
            UserId = null!;
            DisplayName = null!;
        }

        public string UserId { get; init; }
        public string DisplayName { get; init; }
    }
}