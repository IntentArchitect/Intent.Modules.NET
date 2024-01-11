using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(string id, string name, UpdateCustomerAddressDto deliveryAddress)
        {
            Id = id;
            Name = name;
            DeliveryAddress = deliveryAddress;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public UpdateCustomerAddressDto DeliveryAddress { get; set; }
    }
}