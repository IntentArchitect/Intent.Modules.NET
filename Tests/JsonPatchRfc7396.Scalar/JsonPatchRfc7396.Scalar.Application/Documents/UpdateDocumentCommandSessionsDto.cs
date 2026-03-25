using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record UpdateDocumentCommandSessionsDto
    {
        public UpdateDocumentCommandSessionsDto()
        {
            Id = null!;
            ConnectionId = null!;
            CursorJson = null!;
            SelectionJson = null!;
        }

        public string Id { get; init; }
        public string ConnectionId { get; init; }
        public DateTime StartedAtUtc { get; init; }
        public DateTime LastSeenAtUtc { get; init; }
        public string CursorJson { get; init; }
        public string SelectionJson { get; init; }
    }
}