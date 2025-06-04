using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers
{
    public class BuyerDtoProfile : Profile
    {
        public BuyerDtoProfile()
        {
            CreateMap<Buyer, BuyerDto>();
        }
    }

    public static class BuyerDtoMappingExtensions
    {
        public static BuyerDto MapToBuyerDto(this Buyer projectFrom, IMapper mapper) => mapper.Map<BuyerDto>(projectFrom);

        public static List<BuyerDto> MapToBuyerDtoList(this IEnumerable<Buyer> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToBuyerDto(mapper)).ToList();
    }
}