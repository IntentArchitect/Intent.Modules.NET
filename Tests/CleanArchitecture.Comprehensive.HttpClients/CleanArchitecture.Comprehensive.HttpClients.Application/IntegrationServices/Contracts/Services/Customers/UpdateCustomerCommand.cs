using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
{
    public class UpdateCustomerCommand
    {
        public UpdateCustomerCommand()
        {
            Surname = null!;
            Email = null!;
            Address = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public UpdateCustomerAddressDto Address { get; set; }

        public static UpdateCustomerCommand Create(
            Guid id,
            string surname,
            string email,
            UpdateCustomerAddressDto address,
            string name = "default")
        {
            return new UpdateCustomerCommand
            {
                Id = id,
                Surname = surname,
                Email = email,
                Address = address,
                Name = name
            };
        }
    }
}