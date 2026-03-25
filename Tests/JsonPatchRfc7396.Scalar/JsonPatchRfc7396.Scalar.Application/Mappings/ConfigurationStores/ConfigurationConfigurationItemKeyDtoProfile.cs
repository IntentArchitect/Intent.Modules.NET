using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public class ConfigurationConfigurationItemKeyDtoProfile : Profile
    {
        public ConfigurationConfigurationItemKeyDtoProfile()
        {
            CreateMap<ConfigurationKey, ConfigurationConfigurationItemKeyDto>();
        }
    }

    public static class ConfigurationConfigurationItemKeyDtoMappingExtensions
    {
        public static ConfigurationConfigurationItemKeyDto MapToConfigurationConfigurationItemKeyDto(
            this ConfigurationKey projectFrom,
            IMapper mapper) => mapper.Map<ConfigurationConfigurationItemKeyDto>(projectFrom);

        public static List<ConfigurationConfigurationItemKeyDto> MapToConfigurationConfigurationItemKeyDtoList(
            this IEnumerable<ConfigurationKey> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToConfigurationConfigurationItemKeyDto(mapper)).ToList();
    }
}