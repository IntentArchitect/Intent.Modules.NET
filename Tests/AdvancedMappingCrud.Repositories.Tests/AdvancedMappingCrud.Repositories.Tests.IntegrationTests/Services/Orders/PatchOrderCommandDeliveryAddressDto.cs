using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Orders
{
    public class PatchOrderCommandDeliveryAddressDto
    {
        public PatchOrderCommandDeliveryAddressDto()
        {
            Line1 = null!;
        }

        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string? Postal { get; set; }

        public static PatchOrderCommandDeliveryAddressDto Create(string line1, string? line2, string? city, string? postal)
        {
            return new PatchOrderCommandDeliveryAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal
            };
        }
    }
}