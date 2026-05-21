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
        private readonly OrderDiscountDtoMapper _orderDiscountDtoMapper;
        [UseMapper]
        private readonly OrderOrderLineDtoMapper _orderOrderLineDtoMapper;
        [UseMapper]
        private readonly OrderPaymentDtoMapper _orderPaymentDtoMapper;
        [UseMapper]
        private readonly OrderShipmentDtoMapper _orderShipmentDtoMapper;
        [UseMapper]
        private readonly OrderCustomerSegmentsDtoMapper _orderCustomerSegmentsDtoMapper;

        public OrderDtoMapper(OrderDiscountDtoMapper orderDiscountDtoMapper, OrderOrderLineDtoMapper orderOrderLineDtoMapper, OrderPaymentDtoMapper orderPaymentDtoMapper, OrderShipmentDtoMapper orderShipmentDtoMapper, OrderCustomerSegmentsDtoMapper orderCustomerSegmentsDtoMapper)
        {
            _orderDiscountDtoMapper = orderDiscountDtoMapper;
            _orderOrderLineDtoMapper = orderOrderLineDtoMapper;
            _orderPaymentDtoMapper = orderPaymentDtoMapper;
            _orderShipmentDtoMapper = orderShipmentDtoMapper;
            _orderCustomerSegmentsDtoMapper = orderCustomerSegmentsDtoMapper;
        }

        [MapPropertyFromSource(nameof(OrderDto.RequiredBy), Use = nameof(MapRequiredBy))]
        [MapProperty(nameof(Order.Discounts), nameof(OrderDto.Discounts))]
        [MapProperty(nameof(Order.Lines), nameof(OrderDto.OrderLines))]
        [MapProperty(nameof(Order.Payments), nameof(OrderDto.Payments))]
        [MapProperty(nameof(Order.Shipments), nameof(OrderDto.Shipments))]
        [MapPropertyFromSource(nameof(OrderDto.IsActive), Use = nameof(MapIsActive))]
        [MapProperty(nameof(@Order.Customer.CustomerSegments), nameof(OrderDto.CustomerSegments))]
        public partial OrderDto OrderToOrderDto(Order order);

        public partial List<OrderDto> OrderToOrderDtoList(IEnumerable<Order> orders);

        private DateTime MapRequiredBy(Order source) => (DateTime)source.RequiredBy;

        private bool MapIsActive(Order source) => source.IsActive();
    }
}