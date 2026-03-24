using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record UpdateDocumentCommandPermissionsDto
    {
        public UpdateDocumentCommandPermissionsDto()
        {
            Id = null!;
            PrincipalId = null!;
            PrincipalType = null!;
        }

        public string Id { get; init; }
        public DocumentRole Role { get; init; }
        public string PrincipalId { get; init; }
        public string PrincipalType { get; init; }
        public DateTime GrantedAtUtc { get; init; }
    }
}