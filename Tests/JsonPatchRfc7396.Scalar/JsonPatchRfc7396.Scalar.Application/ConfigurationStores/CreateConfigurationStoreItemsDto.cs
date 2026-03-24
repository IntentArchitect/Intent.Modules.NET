using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public record CreateConfigurationStoreItemsDto
    {
        public CreateConfigurationStoreItemsDto()
        {
            Key = null!;
            ScopeKey = null!;
            Value = null!;
        }

        public CreateConfigurationStoreKeyDto Key { get; init; }
        public CreateConfigurationStoreScopeKeyDto ScopeKey { get; init; }
        public ConfigurationValueType ValueType { get; init; }
        public string Value { get; init; }
        public bool IsActive { get; init; }
        public int Version { get; init; }
        public DateTime UpdatedAtUtc { get; init; }
        public Guid? LatestChangeId { get; init; }
    }
}