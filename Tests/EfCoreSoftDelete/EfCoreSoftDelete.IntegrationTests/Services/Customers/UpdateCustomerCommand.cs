using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class UpdateCustomerCommand
    {
        public UpdateCustomerCommand()
        {
            Name = null!;
            OtherAddresses = null!;
            PrimaryAddress = null!;
            OtherBuildings = null!;
            PrimaryBuilding = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UpdateCustomerCommandOtherAddressesDto> OtherAddresses { get; set; }
        public UpdateCustomerPrimaryAddressDto PrimaryAddress { get; set; }
        public List<UpdateCustomerCommandOtherBuildingsDto1> OtherBuildings { get; set; }
        public UpdateCustomerCommandPrimaryBuildingDto1 PrimaryBuilding { get; set; }

        public static UpdateCustomerCommand Create(
            Guid id,
            string name,
            List<UpdateCustomerCommandOtherAddressesDto> otherAddresses,
            UpdateCustomerPrimaryAddressDto primaryAddress,
            List<UpdateCustomerCommandOtherBuildingsDto1> otherBuildings,
            UpdateCustomerCommandPrimaryBuildingDto1 primaryBuilding)
        {
            return new UpdateCustomerCommand
            {
                Id = id,
                Name = name,
                OtherAddresses = otherAddresses,
                PrimaryAddress = primaryAddress,
                OtherBuildings = otherBuildings,
                PrimaryBuilding = primaryBuilding
            };
        }
    }
}