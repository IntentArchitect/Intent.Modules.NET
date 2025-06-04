using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            AddressLine1 = null!;
            AddressCity = null!;
            AddressCountry = null!;
            AddressPostal = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AccountNo { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressPostal { get; set; }

        public static CustomerDto Create(
            Guid id,
            string name,
            string? accountNo,
            string addressLine1,
            string? addressLine2,
            string addressCity,
            string addressCountry,
            string addressPostal)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                AccountNo = accountNo,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                AddressCity = addressCity,
                AddressCountry = addressCountry,
                AddressPostal = addressPostal
            };
        }
    }
}