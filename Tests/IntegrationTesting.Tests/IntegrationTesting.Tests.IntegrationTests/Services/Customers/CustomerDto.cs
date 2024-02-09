using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            AddressLine1 = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }

        public static CustomerDto Create(Guid id, string name, string addressLine1)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                AddressLine1 = addressLine1
            };
        }
    }
}