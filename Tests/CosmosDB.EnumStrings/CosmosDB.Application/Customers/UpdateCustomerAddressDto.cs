using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Customers
{
    public class UpdateCustomerAddressDto
    {
        public UpdateCustomerAddressDto()
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
        public int Id { get; set; }

        public static UpdateCustomerAddressDto Create(string line1, string line2, string city, string postalAddress, int id)
        {
            return new UpdateCustomerAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                PostalAddress = postalAddress,
                Id = id
            };
        }
    }
}