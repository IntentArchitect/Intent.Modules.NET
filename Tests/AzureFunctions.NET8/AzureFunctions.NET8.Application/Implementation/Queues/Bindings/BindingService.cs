using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.Customers;
using AzureFunctions.NET6.Application.Interfaces.Queues.Bindings;
using AzureFunctions.NET8.Application.Customers;
using AzureFunctions.NET8.Application.Interfaces.Queues.Bindings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Implementation.Queues.Bindings
{
    [IntentManaged(Mode.Merge)]
    public class BindingService : IBindingService
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BindingService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CustomerDto> BindingTest(CustomerDto dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}