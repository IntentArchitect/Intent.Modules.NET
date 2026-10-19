using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TestMediator.NoEventing.Domain.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Products
{
    public class PriceHistoryEntryDtoProfile : Profile
    {
        public PriceHistoryEntryDtoProfile()
        {
            CreateMap<PriceHistoryEntry, PriceHistoryEntryDto>();
        }
    }

    public static class PriceHistoryEntryDtoMappingExtensions
    {
        public static PriceHistoryEntryDto MapToPriceHistoryEntryDto(this PriceHistoryEntry projectFrom, IMapper mapper) => mapper.Map<PriceHistoryEntryDto>(projectFrom);

        public static List<PriceHistoryEntryDto> MapToPriceHistoryEntryDtoList(
            this IEnumerable<PriceHistoryEntry> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToPriceHistoryEntryDto(mapper)).ToList();
    }
}