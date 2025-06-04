using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers
{
    public class UpdateBuyerAddressDto
    {
        public UpdateBuyerAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            PostalCode = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public static UpdateBuyerAddressDto Create(string line1, string line2, string city, string postalCode)
        {
            return new UpdateBuyerAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                PostalCode = postalCode
            };
        }
    }
}