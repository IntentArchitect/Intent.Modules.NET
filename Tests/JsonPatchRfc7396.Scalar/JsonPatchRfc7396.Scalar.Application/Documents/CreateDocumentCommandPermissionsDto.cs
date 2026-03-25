using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record CreateDocumentCommandPermissionsDto
    {
        public CreateDocumentCommandPermissionsDto()
        {
            PrincipalId = null!;
            PrincipalType = null!;
        }

        public DocumentRole Role { get; init; }
        public string PrincipalId { get; init; }
        public string PrincipalType { get; init; }
        public DateTime GrantedAtUtc { get; init; }
    }
}