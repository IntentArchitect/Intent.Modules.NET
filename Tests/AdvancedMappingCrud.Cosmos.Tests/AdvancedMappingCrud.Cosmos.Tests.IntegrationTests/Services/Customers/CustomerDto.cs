using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Id = null!;
            Name = null!;
            Surname = null!;
            ShippingAddress = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public CustomerAddressDto ShippingAddress { get; set; }

        public static CustomerDto Create(
            string id,
            string name,
            string surname,
            bool isActive,
            CustomerAddressDto shippingAddress)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                IsActive = isActive,
                ShippingAddress = shippingAddress
            };
        }
    }
}