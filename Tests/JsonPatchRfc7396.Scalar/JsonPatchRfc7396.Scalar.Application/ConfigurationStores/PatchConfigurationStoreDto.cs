using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Application.Common.Patching;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public record PatchConfigurationStoreDto
    {
        public PatchConfigurationStoreDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public List<PatchConfigurationStoreChangesDto>? Changes { get; set; }
        public List<PatchConfigurationStoreItemsDto>? Items { get; set; }
        public required IPatchExecutor<PatchConfigurationStoreDto> PatchExecutor { get; init; }
    }
}