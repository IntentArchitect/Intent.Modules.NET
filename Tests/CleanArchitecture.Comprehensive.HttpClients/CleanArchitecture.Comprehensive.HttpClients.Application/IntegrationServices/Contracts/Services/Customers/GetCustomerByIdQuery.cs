using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
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