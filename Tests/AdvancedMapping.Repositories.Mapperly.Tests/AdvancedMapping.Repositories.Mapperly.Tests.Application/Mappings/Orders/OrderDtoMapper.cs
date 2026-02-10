using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderDtoMapper
    {
        [UseMapper]
        private readonly OrderCustomerDtoMapper _orderCustomerDtoMapper;
        [UseMapper]
        private readonly OrderDiscountDtoMapper _orderDiscountDtoMapper;
        [UseMapper]
        private readonly OrderOrderLineDtoMapper _orderOrderLineDtoMapper;
        [UseMapper]
        private readonly OrderPaymentDtoMapper _orderPaymentDtoMapper;
        [UseMapper]
        private readonly OrderShipmentDtoMapper _orderShipmentDtoMapper;

        public OrderDtoMapper(OrderCustomerDtoMapper orderCustomerDtoMapper, OrderDiscountDtoMapper orderDiscountDtoMapper, OrderOrderLineDtoMapper orderOrderLineDtoMapper, OrderPaymentDtoMapper orderPaymentDtoMapper, OrderShipmentDtoMapper orderShipmentDtoMapper)
        {
            _orderCustomerDtoMapper = orderCustomerDtoMapper;
            _orderDiscountDtoMapper = orderDiscountDtoMapper;
            _orderOrderLineDtoMapper = orderOrderLineDtoMapper;
            _orderPaymentDtoMapper = orderPaymentDtoMapper;
            _orderShipmentDtoMapper = orderShipmentDtoMapper;
        }

        [MapPropertyFromSource(nameof(OrderDto.RequiredBy), Use = nameof(MapRequiredBy))]
        [MapProperty(nameof(Order.Discounts), nameof(OrderDto.Discounts))]
        [MapProperty(nameof(Order.Lines), nameof(OrderDto.OrderLines))]
        [MapProperty(nameof(Order.Payments), nameof(OrderDto.Payments))]
        [MapProperty(nameof(Order.Shipments), nameof(OrderDto.Shipments))]
        [MapPropertyFromSource(nameof(OrderDto.IsActive), Use = nameof(MapIsActive))]
        public partial OrderDto OrderToOrderDto(Order order);

        public partial List<OrderDto> OrderToOrderDtoList(List<Order> orders);

        private DateTime MapRequiredBy(Order source) => (DateTime)source.RequiredBy;

        private bool MapIsActive(Order source) => source.IsActive();
    }
}