using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
{
    public class GetCustomerByNameEmailQuery
    {
        public GetCustomerByNameEmailQuery()
        {
            Name = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Email { get; set; }

        public static GetCustomerByNameEmailQuery Create(string name, string email)
        {
            return new GetCustomerByNameEmailQuery
            {
                Name = name,
                Email = email
            };
        }
    }
}