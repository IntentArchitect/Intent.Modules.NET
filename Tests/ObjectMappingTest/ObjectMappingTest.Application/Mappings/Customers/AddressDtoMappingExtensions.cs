using Intent.RoslynWeaver.Attributes;
using ObjectMappingTest.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ObjectMapping.MappingExtensions", Version = "1.0")]

namespace ObjectMappingTest.Application.Customers
{
    public static class AddressDtoMappingExtensions
    {
        public static AddressDto MapToAddressDto(this Address projectFrom)
        {
            return new AddressDto
            {
                Street = projectFrom.Street,
                City = projectFrom.City,
                PostalCode = projectFrom.PostalCode
            };
        }

        public static List<AddressDto> MapToAddressDtoList(this IEnumerable<Address> projectFrom) => projectFrom.Select(x => x.MapToAddressDto()).ToList();
    }
}