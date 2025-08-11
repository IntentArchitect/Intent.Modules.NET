using EfCoreSoftDelete.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(Guid id,
            string name,
            List<UpdateCustomerCommandOtherAddressesDto> otherAddresses,
            UpdateCustomerPrimaryAddressDto primaryAddress,
            List<UpdateCustomerCommandOtherBuildingsDto1> otherBuildings,
            UpdateCustomerCommandPrimaryBuildingDto1 primaryBuilding)
        {
            Id = id;
            Name = name;
            OtherAddresses = otherAddresses;
            PrimaryAddress = primaryAddress;
            OtherBuildings = otherBuildings;
            PrimaryBuilding = primaryBuilding;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UpdateCustomerCommandOtherAddressesDto> OtherAddresses { get; set; }
        public UpdateCustomerPrimaryAddressDto PrimaryAddress { get; set; }
        public List<UpdateCustomerCommandOtherBuildingsDto1> OtherBuildings { get; set; }
        public UpdateCustomerCommandPrimaryBuildingDto1 PrimaryBuilding { get; set; }
    }
}