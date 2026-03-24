using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record ConfigurationConfigurationItemDto
    {
        public ConfigurationConfigurationItemDto()
        {
            Key = null!;
            ScopeKey = null!;
            Value = null!;
        }

        public Guid Id { get; init; }
        public ConfigurationConfigurationItemKeyDto Key { get; init; }
        public ConfigurationConfigurationItemScopeKeyDto ScopeKey { get; init; }
        public ConfigurationValueType ValueType { get; init; }
        public string Value { get; init; }
        public bool IsActive { get; init; }
        public int Version { get; init; }
        public DateTime UpdatedAtUtc { get; init; }
        public Guid? LatestChangeId { get; init; }
    }
}