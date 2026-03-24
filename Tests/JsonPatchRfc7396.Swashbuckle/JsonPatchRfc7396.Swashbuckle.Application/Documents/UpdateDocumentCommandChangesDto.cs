using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record UpdateDocumentCommandChangesDto
    {
        public UpdateDocumentCommandChangesDto()
        {
            Id = null!;
            PatchJson = null!;
            Actor = null!;
            ClientChangeId = null!;
        }

        public string Id { get; init; }
        public int BaseRevision { get; init; }
        public int ResultingRevision { get; init; }
        public string PatchJson { get; init; }
        public DateTime ChangedAtUtc { get; init; }
        public UpdateDocumentCommandActorDto Actor { get; init; }
        public string ClientChangeId { get; init; }
    }
}