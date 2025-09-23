using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    public class AddressDto
    {
        public AddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Province = null!;
            PostalCode = null!;
            Country = null!;
        }

        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public Guid CustomerId { get; set; }

        public static AddressDto Create(
            Guid id,
            string line1,
            string line2,
            string city,
            string province,
            string postalCode,
            string country,
            Guid customerId)
        {
            return new AddressDto
            {
                Id = id,
                Line1 = line1,
                Line2 = line2,
                City = city,
                Province = province,
                PostalCode = postalCode,
                Country = country,
                CustomerId = customerId
            };
        }
    }
}