using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
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