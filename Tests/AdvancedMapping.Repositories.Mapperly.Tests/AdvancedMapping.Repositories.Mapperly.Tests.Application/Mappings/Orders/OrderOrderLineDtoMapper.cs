using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderOrderLineDtoMapper
    {
        [UseMapper]
        private readonly OrderProductDtoMapper _orderProductDtoMapper;

        public OrderOrderLineDtoMapper(OrderProductDtoMapper orderProductDtoMapper)
        {
            _orderProductDtoMapper = orderProductDtoMapper;
        }

        [MapperIgnoreSource(nameof(OrderLine.OrderId))]
        public partial OrderOrderLineDto OrderLineToOrderOrderLineDto(OrderLine orderLine);

        public partial List<OrderOrderLineDto> OrderLineToOrderOrderLineDtoList(List<OrderLine> orderLines);
    }
}