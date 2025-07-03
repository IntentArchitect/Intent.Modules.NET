using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Email = null!;
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static CustomerDto Create(Guid id, string email, string name, string surname)
        {
            return new CustomerDto
            {
                Id = id,
                Email = email,
                Name = name,
                Surname = surname
            };
        }
    }
}