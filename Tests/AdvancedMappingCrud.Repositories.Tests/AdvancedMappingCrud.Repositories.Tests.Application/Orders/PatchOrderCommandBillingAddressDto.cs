using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders
{
    public class PatchOrderCommandBillingAddressDto
    {
        public PatchOrderCommandBillingAddressDto()
        {
            Line1 = null!;
        }

        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string? Postal { get; set; }

        public static PatchOrderCommandBillingAddressDto Create(string line1, string? line2, string? city, string? postal)
        {
            return new PatchOrderCommandBillingAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal
            };
        }
    }
}