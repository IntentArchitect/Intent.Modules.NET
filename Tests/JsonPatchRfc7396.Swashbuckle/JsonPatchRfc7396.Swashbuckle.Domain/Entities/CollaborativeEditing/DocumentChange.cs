using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing
{
    /// <summary>
    /// A single atomic change applied to a document.
    /// 
    /// Stored as an event-like append-only record: JSON Merge Patch (RFC 7396) + metadata.
    /// </summary>
    public class DocumentChange
    {
        private string? _id;

        public DocumentChange()
        {
            Id = null!;
            PatchJson = null!;
            Actor = null!;
            ClientChangeId = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public int BaseRevision { get; set; }

        public int ResultingRevision { get; set; }

        public string PatchJson { get; set; }

        public DateTime ChangedAtUtc { get; set; }

        public Actor Actor { get; set; }

        public string ClientChangeId { get; set; }
    }
}