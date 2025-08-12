using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Interfaces.Customers
{
    public interface ICustomerService
    {
        Task GetCustomersQuery(CancellationToken cancellationToken = default);
        Task GetCustomerByIdQuery(CancellationToken cancellationToken = default);
        Task UpdateCustomerCommand(CancellationToken cancellationToken = default);
        Task DeleteCustomerCommand(CancellationToken cancellationToken = default);
        Task CreateCustomerCommand(CancellationToken cancellationToken = default);
        Task CreateCustomerNameCommand(string customerName, CancellationToken cancellationToken = default);
    }
}