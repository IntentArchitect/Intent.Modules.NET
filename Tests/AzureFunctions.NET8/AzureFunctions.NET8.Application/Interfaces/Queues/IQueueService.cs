using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Interfaces.Queues
{
    public interface IQueueService : IDisposable
    {
        Task CreateCustomerOp(CustomerDto dto, CancellationToken cancellationToken = default);
        Task CreateCustomerOpWrapped(CustomerDto dto, CancellationToken cancellationToken = default);
    }
}