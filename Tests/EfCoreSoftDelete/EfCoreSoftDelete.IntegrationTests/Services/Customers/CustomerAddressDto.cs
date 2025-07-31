using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class CustomerAddressDto
    {
        public CustomerAddressDto()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Postal = null!;
            OtherBuildings = null!;
            PrimaryBuilding = null!;
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }
        public List<CustomerAddressBuildingDto> OtherBuildings { get; set; }
        public CustomerAddressBuildingDto PrimaryBuilding { get; set; }

        public static CustomerAddressDto Create(
            string line1,
            string line2,
            string city,
            string postal,
            List<CustomerAddressBuildingDto> otherBuildings,
            CustomerAddressBuildingDto primaryBuilding)
        {
            return new CustomerAddressDto
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                Postal = postal,
                OtherBuildings = otherBuildings,
                PrimaryBuilding = primaryBuilding
            };
        }
    }
}