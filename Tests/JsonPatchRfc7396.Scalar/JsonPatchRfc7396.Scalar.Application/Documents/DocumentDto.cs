using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record DocumentDto
    {
        public DocumentDto()
        {
            Id = null!;
            Title = null!;
            Content = null!;
            Changes = null!;
            Permissions = null!;
            Revisions = null!;
            Sessions = null!;
        }

        public string Id { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime UpdatedAtUtc { get; init; }
        public DocumentStatus Status { get; init; }
        public DocumentTitleDto Title { get; init; }
        public DocumentContentDto Content { get; init; }
        public int Revision { get; init; }
        public bool IsDeleted { get; init; }
        public List<DocumentDocumentChangeDto> Changes { get; init; }
        public List<DocumentDocumentPermissionDto> Permissions { get; init; }
        public List<DocumentDocumentRevisionDto> Revisions { get; init; }
        public List<DocumentCollaboratorSessionDto> Sessions { get; init; }
    }
}