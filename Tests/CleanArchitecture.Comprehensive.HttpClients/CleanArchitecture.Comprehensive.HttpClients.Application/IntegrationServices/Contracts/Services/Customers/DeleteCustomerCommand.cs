using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers
{
    public class DeleteCustomerCommand
    {
        public Guid Id { get; set; }

        public static DeleteCustomerCommand Create(Guid id)
        {
            return new DeleteCustomerCommand
            {
                Id = id
            };
        }
    }
}