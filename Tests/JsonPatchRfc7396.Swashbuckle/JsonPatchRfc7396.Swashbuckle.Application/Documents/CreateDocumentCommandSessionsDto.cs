using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record CreateDocumentCommandSessionsDto
    {
        public CreateDocumentCommandSessionsDto()
        {
            ConnectionId = null!;
            CursorJson = null!;
            SelectionJson = null!;
        }

        public string ConnectionId { get; init; }
        public DateTime StartedAtUtc { get; init; }
        public DateTime LastSeenAtUtc { get; init; }
        public object CursorJson { get; init; }
        public object SelectionJson { get; init; }
    }
}