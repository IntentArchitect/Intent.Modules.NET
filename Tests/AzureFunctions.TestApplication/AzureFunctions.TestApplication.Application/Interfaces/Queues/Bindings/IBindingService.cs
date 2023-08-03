using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Interfaces.Queues.Bindings
{
    public interface IBindingService : IDisposable
    {
        Task<CustomerDto> BindingTest(CustomerDto dto, CancellationToken cancellationToken = default);
    }
}