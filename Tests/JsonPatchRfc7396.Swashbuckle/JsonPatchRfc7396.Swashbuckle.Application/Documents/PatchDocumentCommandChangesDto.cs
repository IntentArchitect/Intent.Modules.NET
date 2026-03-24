using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record PatchDocumentCommandChangesDto
    {
        public PatchDocumentCommandChangesDto()
        {
            Id = null!;
            PatchJson = null!;
            Actor = null!;
            ClientChangeId = null!;
        }

        public string Id { get; set; }
        public int BaseRevision { get; set; }
        public int ResultingRevision { get; set; }
        public object PatchJson { get; set; }
        public DateTime ChangedAtUtc { get; set; }
        public PatchDocumentCommandActorDto Actor { get; set; }
        public string ClientChangeId { get; set; }
    }
}