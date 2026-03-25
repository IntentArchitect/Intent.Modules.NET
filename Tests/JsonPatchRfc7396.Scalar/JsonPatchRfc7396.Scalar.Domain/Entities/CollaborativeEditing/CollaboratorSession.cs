using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Entities.CollaborativeEditing
{
    /// <summary>
    /// Tracks a user's active editing session on a document (presence, cursor/selection, etc.).
    /// 
    /// This is typically short-lived and can be TTL'd in Mongo.
    /// </summary>
    public class CollaboratorSession
    {
        private string? _id;

        public CollaboratorSession()
        {
            Id = null!;
            ConnectionId = null!;
            CursorJson = null!;
            SelectionJson = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string ConnectionId { get; set; }

        public DateTime StartedAtUtc { get; set; }

        public DateTime LastSeenAtUtc { get; set; }

        public string CursorJson { get; set; }

        public string SelectionJson { get; set; }
    }
}