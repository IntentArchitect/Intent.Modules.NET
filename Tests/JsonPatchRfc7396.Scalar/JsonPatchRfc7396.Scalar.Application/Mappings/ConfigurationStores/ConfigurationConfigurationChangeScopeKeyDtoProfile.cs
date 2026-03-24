using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public class ConfigurationConfigurationChangeScopeKeyDtoProfile : Profile
    {
        public ConfigurationConfigurationChangeScopeKeyDtoProfile()
        {
            CreateMap<ConfigurationScopeKey, ConfigurationConfigurationChangeScopeKeyDto>();
        }
    }

    public static class ConfigurationConfigurationChangeScopeKeyDtoMappingExtensions
    {
        public static ConfigurationConfigurationChangeScopeKeyDto MapToConfigurationConfigurationChangeScopeKeyDto(
            this ConfigurationScopeKey projectFrom,
            IMapper mapper) => mapper.Map<ConfigurationConfigurationChangeScopeKeyDto>(projectFrom);

        public static List<ConfigurationConfigurationChangeScopeKeyDto> MapToConfigurationConfigurationChangeScopeKeyDtoList(
            this IEnumerable<ConfigurationScopeKey> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToConfigurationConfigurationChangeScopeKeyDto(mapper)).ToList();
    }
}