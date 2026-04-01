using Intent.RoslynWeaver.Attributes;
using SecurityConfig.Tests.Application.IntegrationServices.Contracts.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace SecurityConfig.Tests.Application.IntegrationServices.Contracts.Services.Customers
{
    public class CreateCustomerCommand
    {
        public CreateCustomerCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public CustomerType CustomerType { get; set; }

        public static CreateCustomerCommand Create(string name, string surname, CustomerType customerType)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                Surname = surname,
                CustomerType = customerType
            };
        }
    }
}