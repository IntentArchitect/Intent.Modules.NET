using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.Entities.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.ConfigurationStores
{
    public class ConfigurationStoreDtoProfile : Profile
    {
        public ConfigurationStoreDtoProfile()
        {
            CreateMap<ConfigurationStore, ConfigurationStoreDto>()
                .ForMember(d => d.Changes, opt => opt.MapFrom(src => src.Changes))
                .ForMember(d => d.Items, opt => opt.MapFrom(src => src.Items));
        }
    }

    public static class ConfigurationStoreDtoMappingExtensions
    {
        public static ConfigurationStoreDto MapToConfigurationStoreDto(this ConfigurationStore projectFrom, IMapper mapper) => mapper.Map<ConfigurationStoreDto>(projectFrom);

        public static List<ConfigurationStoreDto> MapToConfigurationStoreDtoList(
            this IEnumerable<ConfigurationStore> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToConfigurationStoreDto(mapper)).ToList();
    }
}