using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Entities.Configuration
{
    /// <summary>
    /// Aggregate root for configuration management.
    /// 
    /// Provides a single aggregate boundary for EF composition relationships.
    /// </summary>
    public class ConfigurationStore
    {
        public ConfigurationStore()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Logical name of the configuration store (e.g. "Default").
        /// </summary>
        public string Name { get; set; }

        public virtual ICollection<ConfigurationItem>? Items { get; set; } = [];

        public virtual ICollection<ConfigurationChange>? Changes { get; set; } = [];
    }
}