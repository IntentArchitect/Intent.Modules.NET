using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers
{
    public class DeleteCustomerCommand
    {
        public Guid Id { get; set; }

        public static DeleteCustomerCommand Create(Guid id)
        {
            return new DeleteCustomerCommand
            {
                Id = id
            };
        }
    }
}