using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public record PatchConfigurationStoreItemsDto
    {
        public PatchConfigurationStoreItemsDto()
        {
            Key = null!;
            ScopeKey = null!;
            Value = null!;
        }

        public Guid Id { get; set; }
        public PatchConfigurationStoreKeyDto Key { get; set; }
        public PatchConfigurationStoreScopeKeyDto ScopeKey { get; set; }
        public ConfigurationValueType ValueType { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public int Version { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public Guid? LatestChangeId { get; set; }
    }
}