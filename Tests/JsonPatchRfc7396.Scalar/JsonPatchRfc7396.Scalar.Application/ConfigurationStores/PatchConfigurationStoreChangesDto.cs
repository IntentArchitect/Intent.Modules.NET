using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public record PatchConfigurationStoreChangesDto
    {
        public PatchConfigurationStoreChangesDto()
        {
            Key = null!;
            ScopeKey = null!;
            ChangedBy = null!;
        }

        public Guid Id { get; set; }
        public PatchConfigurationStoreKeyDto Key { get; set; }
        public PatchConfigurationStoreScopeKeyDto ScopeKey { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangedAtUtc { get; set; }
        public string ChangedBy { get; set; }
    }
}