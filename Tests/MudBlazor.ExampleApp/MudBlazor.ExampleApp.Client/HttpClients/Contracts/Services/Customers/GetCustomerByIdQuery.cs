using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers
{
    public class GetCustomerByIdQuery
    {
        public Guid Id { get; set; }

        public static GetCustomerByIdQuery Create(Guid id)
        {
            return new GetCustomerByIdQuery
            {
                Id = id
            };
        }
    }
}