using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record CreateDocumentCommandRevisionsDto
    {
        public CreateDocumentCommandRevisionsDto()
        {
            Content = null!;
            Author = null!;
        }

        public int Revision { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public CreateDocumentCommandContentDto Content { get; init; }
        public CreateDocumentCommandActorDto Author { get; init; }
    }
}