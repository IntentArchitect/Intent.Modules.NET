using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<string>, ICommand
    {
        public CreateCustomerCommand(string name, CreateCustomerAddressDto deliveryAddress)
        {
            Name = name;
            DeliveryAddress = deliveryAddress;
        }

        public string Name { get; set; }
        public CreateCustomerAddressDto DeliveryAddress { get; set; }
    }
}