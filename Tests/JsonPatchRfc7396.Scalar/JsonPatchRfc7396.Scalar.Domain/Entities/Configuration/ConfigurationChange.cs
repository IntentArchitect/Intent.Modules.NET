using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Entities.Configuration
{
    /// <summary>
    /// Append-only audit log of configuration changes.
    /// </summary>
    public class ConfigurationChange
    {
        public ConfigurationChange()
        {
            Key = null!;
            ScopeKey = null!;
            ChangedBy = null!;
        }

        public Guid Id { get; set; }

        public ConfigurationKey Key { get; set; }

        public ConfigurationScopeKey ScopeKey { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime ChangedAtUtc { get; set; }

        /// <summary>
        /// Identifier of the actor making the change (e.g. user id / service principal).
        /// </summary>
        public string ChangedBy { get; set; }

        public Guid ConfigurationStoreId { get; set; }
    }
}