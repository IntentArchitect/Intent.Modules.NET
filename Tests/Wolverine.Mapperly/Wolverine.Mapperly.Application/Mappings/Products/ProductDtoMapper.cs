using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;
using Wolverine.Mapperly.Application.Products;
using Wolverine.Mapperly.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace Wolverine.Mapperly.Application.Mappings.Products
{
    [Mapper]
    public partial class ProductDtoMapper
    {
        [MapperIgnoreSource(nameof(Product.DomainEvents))]
        public partial ProductDto ProductToProductDto(Product product);

        public partial List<ProductDto> ProductToProductDtoList(IEnumerable<Product> products);
    }
}