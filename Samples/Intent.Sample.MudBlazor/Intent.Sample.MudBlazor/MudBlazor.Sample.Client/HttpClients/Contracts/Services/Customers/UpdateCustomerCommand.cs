using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Customers
{
    public class UpdateCustomerCommand
    {
        public UpdateCustomerCommand()
        {
            Name = null!;
            Address = new();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public UpdateCustomerAddressDto Address { get; set; }

        public static UpdateCustomerCommand Create(Guid id, string name, string? accountNo, UpdateCustomerAddressDto address)
        {
            return new UpdateCustomerCommand
            {
                Id = id,
                Name = name,
                AccountNo = accountNo,
                Address = address
            };
        }
    }
}