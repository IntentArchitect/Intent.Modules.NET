using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public class ConfigurationConfigurationItemScopeKeyDtoProfile : Profile
    {
        public ConfigurationConfigurationItemScopeKeyDtoProfile()
        {
            CreateMap<ConfigurationScopeKey, ConfigurationConfigurationItemScopeKeyDto>();
        }
    }

    public static class ConfigurationConfigurationItemScopeKeyDtoMappingExtensions
    {
        public static ConfigurationConfigurationItemScopeKeyDto MapToConfigurationConfigurationItemScopeKeyDto(
            this ConfigurationScopeKey projectFrom,
            IMapper mapper) => mapper.Map<ConfigurationConfigurationItemScopeKeyDto>(projectFrom);

        public static List<ConfigurationConfigurationItemScopeKeyDto> MapToConfigurationConfigurationItemScopeKeyDtoList(
            this IEnumerable<ConfigurationScopeKey> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToConfigurationConfigurationItemScopeKeyDto(mapper)).ToList();
    }
}