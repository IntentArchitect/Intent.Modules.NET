using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            OtherAddresses = null!;
            PrimaryAddress = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public List<CustomerAddressDto> OtherAddresses { get; set; }
        public CustomerAddressDto PrimaryAddress { get; set; }

        public static CustomerDto Create(
            Guid id,
            string name,
            bool isDeleted,
            List<CustomerAddressDto> otherAddresses,
            CustomerAddressDto primaryAddress)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                IsDeleted = isDeleted,
                OtherAddresses = otherAddresses,
                PrimaryAddress = primaryAddress
            };
        }
    }
}