using EfCoreSoftDelete.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name,
            List<CreateCustomerCommandOtherAddressesDto> otherAddresses,
            CreateCustomerPrimaryAddressDto primaryAddress,
            List<CreateCustomerCommandOtherBuildingsDto1> otherBuildings,
            CreateCustomerCommandPrimaryBuildingDto1 primaryBuilding)
        {
            Name = name;
            OtherAddresses = otherAddresses;
            PrimaryAddress = primaryAddress;
            OtherBuildings = otherBuildings;
            PrimaryBuilding = primaryBuilding;
        }

        public string Name { get; set; }
        public List<CreateCustomerCommandOtherAddressesDto> OtherAddresses { get; set; }
        public CreateCustomerPrimaryAddressDto PrimaryAddress { get; set; }
        public List<CreateCustomerCommandOtherBuildingsDto1> OtherBuildings { get; set; }
        public CreateCustomerCommandPrimaryBuildingDto1 PrimaryBuilding { get; set; }
    }
}