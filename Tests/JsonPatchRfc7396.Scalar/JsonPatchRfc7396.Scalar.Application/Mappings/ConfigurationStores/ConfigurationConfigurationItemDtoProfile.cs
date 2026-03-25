using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Entities.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public class ConfigurationConfigurationItemDtoProfile : Profile
    {
        public ConfigurationConfigurationItemDtoProfile()
        {
            CreateMap<ConfigurationItem, ConfigurationConfigurationItemDto>();
        }
    }

    public static class ConfigurationConfigurationItemDtoMappingExtensions
    {
        public static ConfigurationConfigurationItemDto MapToConfigurationConfigurationItemDto(
            this ConfigurationItem projectFrom,
            IMapper mapper) => mapper.Map<ConfigurationConfigurationItemDto>(projectFrom);

        public static List<ConfigurationConfigurationItemDto> MapToConfigurationConfigurationItemDtoList(
            this IEnumerable<ConfigurationItem> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToConfigurationConfigurationItemDto(mapper)).ToList();
    }
}