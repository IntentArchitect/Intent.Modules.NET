using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing
{
    /// <summary>
    /// Immutable snapshot of a document at a point in time.
    /// 
    /// Useful for audit/history and for rebuilding derived read models.
    /// </summary>
    public class DocumentRevision
    {
        private string? _id;

        public DocumentRevision()
        {
            Id = null!;
            Content = null!;
            Author = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public int Revision { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DocumentContent Content { get; set; }

        public Actor Author { get; set; }
    }
}