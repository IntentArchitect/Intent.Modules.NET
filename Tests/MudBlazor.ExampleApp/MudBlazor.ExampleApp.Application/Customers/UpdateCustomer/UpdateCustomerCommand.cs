using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(Guid id, string name, string? accountNo, UpdateCustomerAddressDto address)
        {
            Id = id;
            Name = name;
            AccountNo = accountNo;
            Address = address;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public UpdateCustomerAddressDto Address { get; set; }
    }
}