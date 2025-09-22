using AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers
{
    [Mapper]
    public partial class AddressDtoMapper
    {
        public partial AddressDto AddressToAddressDto(Address Address);

        public partial List<AddressDto> AddressToAddressDtoList(List<Address> Addresses);
    }
}