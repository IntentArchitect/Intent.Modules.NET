using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
{
    public class UpdateCustomerCommandOtherAddressesDto
    {
        public UpdateCustomerCommandOtherAddressesDto()
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
        public List<UpdateCustomerCommandOtherBuildingsDto> OtherBuildings { get; set; }
        public UpdateCustomerCommandPrimaryBuildingDto PrimaryBuilding { get; set; }

        public static UpdateCustomerCommandOtherAddressesDto Create(
            string line1,
            string line2,
            string city,
            string postal,
            List<UpdateCustomerCommandOtherBuildingsDto> otherBuildings,
            UpdateCustomerCommandPrimaryBuildingDto primaryBuilding)
        {
            return new UpdateCustomerCommandOtherAddressesDto
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