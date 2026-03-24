using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record UpdateDocumentCommandRevisionsDto
    {
        public UpdateDocumentCommandRevisionsDto()
        {
            Id = null!;
            Content = null!;
            Author = null!;
        }

        public string Id { get; init; }
        public int Revision { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public UpdateDocumentCommandContentDto Content { get; init; }
        public UpdateDocumentCommandActorDto Author { get; init; }
    }
}