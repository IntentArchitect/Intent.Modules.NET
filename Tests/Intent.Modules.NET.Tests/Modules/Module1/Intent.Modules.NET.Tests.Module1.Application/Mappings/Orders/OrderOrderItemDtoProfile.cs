using AutoMapper;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders;
using Intent.Modules.NET.Tests.Module1.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Orders
{
    public class OrderOrderItemDtoProfile : Profile
    {
        public OrderOrderItemDtoProfile()
        {
            CreateMap<OrderItem, OrderOrderItemDto>();
        }
    }

    public static class OrderOrderItemDtoMappingExtensions
    {
        public static OrderOrderItemDto MapToOrderOrderItemDto(this OrderItem projectFrom, IMapper mapper) => mapper.Map<OrderOrderItemDto>(projectFrom);

        public static List<OrderOrderItemDto> MapToOrderOrderItemDtoList(
            this IEnumerable<OrderItem> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToOrderOrderItemDto(mapper)).ToList();
    }
}