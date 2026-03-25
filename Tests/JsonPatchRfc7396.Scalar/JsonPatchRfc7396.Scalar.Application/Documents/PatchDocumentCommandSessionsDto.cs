using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record PatchDocumentCommandSessionsDto
    {
        public PatchDocumentCommandSessionsDto()
        {
            Id = null!;
            ConnectionId = null!;
            CursorJson = null!;
            SelectionJson = null!;
        }

        public string Id { get; set; }
        public string ConnectionId { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime LastSeenAtUtc { get; set; }
        public string CursorJson { get; set; }
        public string SelectionJson { get; set; }
    }
}