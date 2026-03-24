using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record UpdateConfigurationStoreDto
    {
        public UpdateConfigurationStoreDto()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public List<UpdateConfigurationStoreChangesDto>? Changes { get; init; }
        public List<UpdateConfigurationStoreItemsDto>? Items { get; init; }
    }
}