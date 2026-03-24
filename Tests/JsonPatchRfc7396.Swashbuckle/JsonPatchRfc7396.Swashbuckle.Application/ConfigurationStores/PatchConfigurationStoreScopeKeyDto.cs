using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public record PatchConfigurationStoreScopeKeyDto
    {
        public PatchConfigurationStoreScopeKeyDto()
        {
        }

        public ConfigurationScope Scope { get; set; }
        public string? ScopeId { get; set; }
    }
}