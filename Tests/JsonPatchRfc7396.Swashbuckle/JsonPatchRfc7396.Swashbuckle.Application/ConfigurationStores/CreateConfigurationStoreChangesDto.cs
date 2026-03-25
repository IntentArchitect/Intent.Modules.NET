using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record CreateConfigurationStoreChangesDto
    {
        public CreateConfigurationStoreChangesDto()
        {
            Key = null!;
            ScopeKey = null!;
            ChangedBy = null!;
        }

        public CreateConfigurationStoreKeyDto Key { get; init; }
        public CreateConfigurationStoreScopeKeyDto ScopeKey { get; init; }
        public string? OldValue { get; init; }
        public string? NewValue { get; init; }
        public DateTime ChangedAtUtc { get; init; }
        public string ChangedBy { get; init; }
    }
}