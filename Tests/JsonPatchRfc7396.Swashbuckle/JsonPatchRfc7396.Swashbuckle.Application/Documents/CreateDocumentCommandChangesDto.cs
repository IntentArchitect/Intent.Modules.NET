using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record CreateDocumentCommandChangesDto
    {
        public CreateDocumentCommandChangesDto()
        {
            PatchJson = null!;
            Actor = null!;
            ClientChangeId = null!;
        }

        public int BaseRevision { get; init; }
        public int ResultingRevision { get; init; }
        public object PatchJson { get; init; }
        public DateTime ChangedAtUtc { get; init; }
        public CreateDocumentCommandActorDto Actor { get; init; }
        public string ClientChangeId { get; init; }
    }
}