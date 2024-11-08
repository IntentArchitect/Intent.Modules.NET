using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers
{
    public class CreateCustomerCommand
    {
        public CreateCustomerCommand()
        {
            Name = null!;
            Address = new();
        }

        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public CreateCustomerAddressDto Address { get; set; }

        public static CreateCustomerCommand Create(string name, string? accountNo, CreateCustomerAddressDto address)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                AccountNo = accountNo,
                Address = address
            };
        }
    }
}