using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderProductDtoMapper
    {
        [UseMapper]
        private readonly OrderProductCategoryDtoMapper _orderProductCategoryDtoMapper;

        public OrderProductDtoMapper(OrderProductCategoryDtoMapper orderProductCategoryDtoMapper)
        {
            _orderProductCategoryDtoMapper = orderProductCategoryDtoMapper;
        }

        public partial OrderProductDto ProductToOrderProductDto(Product product);

        public partial List<OrderProductDto> ProductToOrderProductDtoList(List<Product> products);
    }
}