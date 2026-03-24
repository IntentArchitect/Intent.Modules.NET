using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public record ConfigurationStoreDto
    {
        public ConfigurationStoreDto()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public List<ConfigurationConfigurationChangeDto>? Changes { get; init; }
        public List<ConfigurationConfigurationItemDto>? Items { get; init; }
    }
}