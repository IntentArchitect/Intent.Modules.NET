using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record CreateConfigurationStoreDto
    {
        public CreateConfigurationStoreDto()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public List<CreateConfigurationStoreChangesDto>? Changes { get; init; }
        public List<CreateConfigurationStoreItemsDto>? Items { get; init; }
    }
}