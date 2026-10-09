using AutoMapper;
using CqrsSpike.Publisher.Domain.Entities;

namespace CqrsSpike.Publisher.Application.Orders
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