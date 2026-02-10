using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class OrderAddressDto
    {
        public OrderAddressDto()
        {
            Line1 = null!;
            City = null!;
            PostCode = null!;
        }

        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }

        public static OrderAddressDto Create(Guid id, string line1, string? line2, string city, string postCode)
        {
            return new OrderAddressDto
            {
                Id = id,
                Line1 = line1,
                Line2 = line2,
                City = city,
                PostCode = postCode
            };
        }
    }
}