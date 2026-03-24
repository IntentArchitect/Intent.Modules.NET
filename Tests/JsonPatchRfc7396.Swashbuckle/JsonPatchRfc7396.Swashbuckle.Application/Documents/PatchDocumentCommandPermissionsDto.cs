using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record PatchDocumentCommandPermissionsDto
    {
        public PatchDocumentCommandPermissionsDto()
        {
            Id = null!;
            PrincipalId = null!;
            PrincipalType = null!;
        }

        public string Id { get; set; }
        public DocumentRole Role { get; set; }
        public string PrincipalId { get; set; }
        public string PrincipalType { get; set; }
        public DateTime GrantedAtUtc { get; set; }
    }
}