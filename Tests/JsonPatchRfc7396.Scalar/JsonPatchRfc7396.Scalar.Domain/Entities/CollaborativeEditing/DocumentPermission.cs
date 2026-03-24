using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Entities.CollaborativeEditing
{
    /// <summary>
    /// Grants a principal a role on a document.
    /// 
    /// Modeled as a separate entity to allow multiple principals per document.
    /// </summary>
    public class DocumentPermission
    {
        private string? _id;

        public DocumentPermission()
        {
            Id = null!;
            PrincipalId = null!;
            PrincipalType = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public DocumentRole Role { get; set; }

        public string PrincipalId { get; set; }

        public string PrincipalType { get; set; }

        public DateTime GrantedAtUtc { get; set; }
    }
}