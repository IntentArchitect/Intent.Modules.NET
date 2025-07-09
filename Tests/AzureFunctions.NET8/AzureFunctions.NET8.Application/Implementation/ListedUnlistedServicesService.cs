using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET8.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ListedUnlistedServicesService : IListedUnlistedServicesService
    {
        [IntentManaged(Mode.Merge)]
        public ListedUnlistedServicesService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ListedServiceFunc(string param, CancellationToken cancellationToken = default)
        {
            // TODO: Implement ListedServiceFunc (ListedUnlistedServicesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UnlistedServiceFunc(CancellationToken cancellationToken = default)
        {
            // TODO: Implement UnlistedServiceFunc (ListedUnlistedServicesService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}