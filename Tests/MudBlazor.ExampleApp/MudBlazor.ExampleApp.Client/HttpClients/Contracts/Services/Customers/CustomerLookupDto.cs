using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers
{
    public class CustomerLookupDto
    {
        public CustomerLookupDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CustomerLookupDto Create(Guid id, string name)
        {
            return new CustomerLookupDto
            {
                Id = id,
                Name = name
            };
        }
    }
}