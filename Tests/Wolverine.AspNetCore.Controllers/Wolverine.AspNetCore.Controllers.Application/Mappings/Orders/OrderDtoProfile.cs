using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application
{
    public class OrderDtoProfile : Profile
    {
        public OrderDtoProfile()
        {
            CreateMap<Order, OrderDto>();
        }
    }

    public static class OrderDtoMappingExtensions
    {
        public static OrderDto MapToOrderDto(this Order projectFrom, IMapper mapper) => mapper.Map<OrderDto>(projectFrom);

        public static List<OrderDto> MapToOrderDtoList(this IEnumerable<Order> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToOrderDto(mapper)).ToList();
    }
}