using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Entities.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Application.Items
{
    public class ItemDtoProfile : Profile
    {
        public ItemDtoProfile()
        {
            CreateMap<Item, ItemDto>();
        }
    }

    public static class ItemDtoMappingExtensions
    {
        public static ItemDto MapToItemDto(this Item projectFrom, IMapper mapper) => mapper.Map<ItemDto>(projectFrom);

        public static List<ItemDto> MapToItemDtoList(this IEnumerable<Item> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToItemDto(mapper)).ToList();
    }
}