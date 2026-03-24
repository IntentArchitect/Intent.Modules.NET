using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public record CreateConfigurationStoreScopeKeyDto
    {
        public CreateConfigurationStoreScopeKeyDto()
        {
        }

        public ConfigurationScope Scope { get; init; }
        public string? ScopeId { get; init; }
    }
}