using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderCustomerDtoMapper
    {
        [UseMapper]
        private readonly OrderAddressDtoMapper _orderAddressDtoMapper;

        public OrderCustomerDtoMapper(OrderAddressDtoMapper orderAddressDtoMapper)
        {
            _orderAddressDtoMapper = orderAddressDtoMapper;
        }

        [MapProperty(nameof(Customer.Addresses), nameof(OrderCustomerDto.Addresses))]
        public partial OrderCustomerDto CustomerToOrderCustomerDto(Customer customer);

        public partial List<OrderCustomerDto> CustomerToOrderCustomerDtoList(List<Customer> customers);
    }
}