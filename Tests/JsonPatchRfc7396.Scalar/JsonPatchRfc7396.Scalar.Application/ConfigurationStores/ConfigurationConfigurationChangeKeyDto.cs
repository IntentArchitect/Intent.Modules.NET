using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public record ConfigurationConfigurationChangeKeyDto
    {
        public ConfigurationConfigurationChangeKeyDto()
        {
            Value = null!;
        }

        public string Value { get; init; }
    }
}