using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record PatchConfigurationItemKeyDto
    {
        public PatchConfigurationItemKeyDto()
        {
            Value = null!;
        }

        public string Value { get; init; }
    }
}