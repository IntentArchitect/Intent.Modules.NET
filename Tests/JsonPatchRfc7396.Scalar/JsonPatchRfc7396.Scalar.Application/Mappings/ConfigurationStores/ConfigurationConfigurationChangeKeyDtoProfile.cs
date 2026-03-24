using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public class ConfigurationConfigurationChangeKeyDtoProfile : Profile
    {
        public ConfigurationConfigurationChangeKeyDtoProfile()
        {
            CreateMap<ConfigurationKey, ConfigurationConfigurationChangeKeyDto>();
        }
    }

    public static class ConfigurationConfigurationChangeKeyDtoMappingExtensions
    {
        public static ConfigurationConfigurationChangeKeyDto MapToConfigurationConfigurationChangeKeyDto(
            this ConfigurationKey projectFrom,
            IMapper mapper) => mapper.Map<ConfigurationConfigurationChangeKeyDto>(projectFrom);

        public static List<ConfigurationConfigurationChangeKeyDto> MapToConfigurationConfigurationChangeKeyDtoList(
            this IEnumerable<ConfigurationKey> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToConfigurationConfigurationChangeKeyDto(mapper)).ToList();
    }
}