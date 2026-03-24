using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record ConfigurationConfigurationChangeDto
    {
        public ConfigurationConfigurationChangeDto()
        {
            Key = null!;
            ScopeKey = null!;
            ChangedBy = null!;
        }

        public Guid Id { get; init; }
        public ConfigurationConfigurationChangeKeyDto Key { get; init; }
        public ConfigurationConfigurationChangeScopeKeyDto ScopeKey { get; init; }
        public string? OldValue { get; init; }
        public string? NewValue { get; init; }
        public DateTime ChangedAtUtc { get; init; }
        public string ChangedBy { get; init; }
    }
}