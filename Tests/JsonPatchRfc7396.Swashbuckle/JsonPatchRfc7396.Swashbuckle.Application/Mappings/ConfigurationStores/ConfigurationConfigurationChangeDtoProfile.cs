using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores
{
    public class ConfigurationConfigurationChangeDtoProfile : Profile
    {
        public ConfigurationConfigurationChangeDtoProfile()
        {
            CreateMap<ConfigurationChange, ConfigurationConfigurationChangeDto>();
        }
    }

    public static class ConfigurationConfigurationChangeDtoMappingExtensions
    {
        public static ConfigurationConfigurationChangeDto MapToConfigurationConfigurationChangeDto(
            this ConfigurationChange projectFrom,
            IMapper mapper) => mapper.Map<ConfigurationConfigurationChangeDto>(projectFrom);

        public static List<ConfigurationConfigurationChangeDto> MapToConfigurationConfigurationChangeDtoList(
            this IEnumerable<ConfigurationChange> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToConfigurationConfigurationChangeDto(mapper)).ToList();
    }
}