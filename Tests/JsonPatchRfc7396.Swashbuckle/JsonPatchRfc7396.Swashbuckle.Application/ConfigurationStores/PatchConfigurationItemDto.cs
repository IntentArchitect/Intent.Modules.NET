using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.Common.Patching;
using JsonPatchRfc7396.Swashbuckle.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record PatchConfigurationItemDto
    {
        public PatchConfigurationItemDto()
        {
            Key = null!;
            ScopeKey = null!;
            Value = null!;
        }

        public PatchConfigurationItemKeyDto1 Key { get; set; }
        public PatchConfigurationItemScopeKeyDto1 ScopeKey { get; set; }
        public ConfigurationValueType ValueType { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public int Version { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public Guid? LatestChangeId { get; set; }
        public required IPatchExecutor<PatchConfigurationItemDto> PatchExecutor { get; init; }
    }
}