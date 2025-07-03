using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
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
        public string Email { get; set; }
        public CreateCustomerAddressDto Address { get; set; }

        public static CreateCustomerCommand Create(
            string name,
            string surname,
            string email = "",
            CreateCustomerAddressDto address = null)
        {
            return new CreateCustomerCommand
            {
                Name = name,
                Surname = surname,
                Email = email,
                Address = address
            };
        }
    }
}