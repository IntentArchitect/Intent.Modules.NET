using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Customers
{
    public class CreateCustomerAddressDto
    {
        public CreateCustomerAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            PostalAddress = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalAddress { get; set; }

        public static CreateCustomerAddressDto Create(string line1, string line2, string city, string postalAddress)
        {
            return new CreateCustomerAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                PostalAddress = postalAddress
            };
        }
    }
}