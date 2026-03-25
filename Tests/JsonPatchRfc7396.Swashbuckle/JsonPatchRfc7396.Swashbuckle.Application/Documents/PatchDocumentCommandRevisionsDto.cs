using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record PatchDocumentCommandRevisionsDto
    {
        public PatchDocumentCommandRevisionsDto()
        {
            Id = null!;
            Content = null!;
            Author = null!;
        }

        public string Id { get; set; }
        public int Revision { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public PatchDocumentCommandContentDto Content { get; set; }
        public PatchDocumentCommandActorDto Author { get; set; }
    }
}