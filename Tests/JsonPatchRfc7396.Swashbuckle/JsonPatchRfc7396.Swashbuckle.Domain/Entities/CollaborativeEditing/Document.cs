using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing
{
    /// <summary>
    /// Represents a collaboratively edited document.
    /// 
    /// Stores the latest materialized content plus collaboration metadata (ETag/revision).
    /// </summary>
    public class Document
    {
        public Document()
        {
            Id = null!;
            Title = null!;
            Content = null!;
        }

        public string Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public DocumentStatus Status { get; set; }

        public DocumentTitle Title { get; set; }

        public DocumentContent Content { get; set; }

        public int Revision { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<DocumentRevision> Revisions { get; set; } = [];

        public ICollection<DocumentChange> Changes { get; set; } = [];

        public ICollection<CollaboratorSession> Sessions { get; set; } = [];

        public ICollection<DocumentPermission> Permissions { get; set; } = [];
    }
}