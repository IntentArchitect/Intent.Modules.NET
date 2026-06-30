using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Customers
{
    public record CustomerDetailDto
    {
        public CustomerDetailDto()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string? Email { get; init; }
        public AddressDto? Address { get; init; }
        public AddressDto? ShippingAddress { get; init; }
    }
}