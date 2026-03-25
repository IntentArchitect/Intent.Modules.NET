using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record DocumentDocumentRevisionDto
    {
        public DocumentDocumentRevisionDto()
        {
            Id = null!;
            Content = null!;
            Author = null!;
        }

        public string Id { get; init; }
        public int Revision { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DocumentDocumentRevisionContentDto Content { get; init; }
        public DocumentDocumentRevisionAuthorDto Author { get; init; }
    }
}