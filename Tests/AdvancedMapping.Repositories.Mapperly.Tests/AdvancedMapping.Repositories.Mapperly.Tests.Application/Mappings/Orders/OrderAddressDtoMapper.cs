using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class OrderAddressDtoMapper
    {
        [MapperIgnoreSource(nameof(Address.CustomerId))]
        public partial OrderAddressDto AddressToOrderAddressDto(Address address);

        public partial List<OrderAddressDto> AddressToOrderAddressDtoList(List<Address> addresses);
    }
}