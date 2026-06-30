using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Customers
{
    public record AddressDto
    {
        public AddressDto()
        {
            Street = null!;
            City = null!;
            PostalCode = null!;
        }

        public string Street { get; init; }
        public string City { get; init; }
        public string PostalCode { get; init; }
    }
}