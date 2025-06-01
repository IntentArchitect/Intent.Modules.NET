using AutoMapper;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders;
using Intent.Modules.NET.Tests.Module1.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Orders
{
    public class OrderDtoProfile : Profile
    {
        public OrderDtoProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
        }
    }

    public static class OrderDtoMappingExtensions
    {
        public static OrderDto MapToOrderDto(this Order projectFrom, IMapper mapper) => mapper.Map<OrderDto>(projectFrom);

        public static List<OrderDto> MapToOrderDtoList(this IEnumerable<Order> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToOrderDto(mapper)).ToList();
    }
}