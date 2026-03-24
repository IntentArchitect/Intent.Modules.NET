using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
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
        public object CursorJson { get; set; }
        public object SelectionJson { get; set; }
    }
}