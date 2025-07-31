using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class CreateCustomerCommand
    {
        public CreateCustomerCommand()
        {
            Name = null!;
            OtherAddresses = null!;
            PrimaryAddress = null!;
            OtherBuildings = null!;
            PrimaryBuilding = null!;
        }

        public string Name { get; set; }
        public List<CreateCustomerCommandOtherAddressesDto> OtherAddresses { get; set; }
        public CreateCustomerPrimaryAddressDto PrimaryAddress { get; set; }
        public List<CreateCustomerCommandOtherBuildingsDto1> OtherBuildings { get; set; }
        public CreateCustomerCommandPrimaryBuildingDto1 PrimaryBuilding { get; set; }

        public static CreateCustomerCommand Create(
            string name,
            List<CreateCustomerCommandOtherAddressesDto> otherAddresses,
            CreateCustomerPrimaryAddressDto primaryAddress,
            List<CreateCustomerCommandOtherBuildingsDto1> otherBuildings,
            CreateCustomerCommandPrimaryBuildingDto1 primaryBuilding)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                OtherAddresses = otherAddresses,
                PrimaryAddress = primaryAddress,
                OtherBuildings = otherBuildings,
                PrimaryBuilding = primaryBuilding
            };
        }
    }
}