using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Configuration;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Entities.Configuration
{
    /// <summary>
    /// Stores a single configuration value for a key, optionally scoped.
    /// 
    /// Do not store secrets in plain text; use an external secret store and store a reference.
    /// </summary>
    public class ConfigurationItem
    {
        public ConfigurationItem()
        {
            Key = null!;
            ScopeKey = null!;
            Value = null!;
        }

        public Guid Id { get; set; }

        public ConfigurationKey Key { get; set; }

        public ConfigurationScopeKey ScopeKey { get; set; }

        public ConfigurationValueType ValueType { get; set; }

        /// <summary>
        /// Serialized representation of the value. Interpretation depends on `ValueType`.
        /// </summary>
        public string Value { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Monotonically increasing version for cache invalidation / optimistic concurrency.
        /// </summary>
        public int Version { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Guid ConfigurationStoreId { get; set; }

        public Guid? LatestChangeId { get; set; }

        public virtual ConfigurationChange? LatestChange { get; set; }
    }
}