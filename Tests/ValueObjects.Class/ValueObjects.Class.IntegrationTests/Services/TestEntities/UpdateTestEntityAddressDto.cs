using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace ValueObjects.Class.IntegrationTests.Services.TestEntities
{
    public class UpdateTestEntityAddressDto
    {
        public UpdateTestEntityAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Country = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }

        public static UpdateTestEntityAddressDto Create(
            string line1,
            string line2,
            string city,
            string country,
            AddressType addressType)
        {
            return new UpdateTestEntityAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Country = country,
                AddressType = addressType
            };
        }
    }
}