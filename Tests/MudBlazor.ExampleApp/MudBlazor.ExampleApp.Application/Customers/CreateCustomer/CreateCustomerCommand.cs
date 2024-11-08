using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string? accountNo, CreateCustomerAddressDto address)
        {
            Name = name;
            AccountNo = accountNo;
            Address = address;
        }

        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public CreateCustomerAddressDto Address { get; set; }
    }
}