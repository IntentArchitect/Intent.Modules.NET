using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderProductCategoryDtoMapper
    {
        public partial OrderProductCategoryDto ProductCategoryToOrderProductCategoryDto(ProductCategory productCategory);

        public partial List<OrderProductCategoryDto> ProductCategoryToOrderProductCategoryDtoList(List<ProductCategory> productCategories);
    }
}