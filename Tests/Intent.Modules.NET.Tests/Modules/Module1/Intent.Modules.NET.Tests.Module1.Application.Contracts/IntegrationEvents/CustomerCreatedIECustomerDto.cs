using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Module1.Eventing.Messages
{
    public class CustomerCreatedIECustomerDto
    {
        public CustomerCreatedIECustomerDto()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CustomerCreatedIECustomerDto Create(Guid id, string name)
        {
            return new CustomerCreatedIECustomerDto
            {
                Id = id,
                Name = name
            };
        }
    }
}