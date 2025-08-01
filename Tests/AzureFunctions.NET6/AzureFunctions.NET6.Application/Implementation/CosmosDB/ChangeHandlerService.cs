using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.CosmosDB;
using AzureFunctions.NET6.Application.Interfaces.CosmosDB;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Implementation.CosmosDB
{
    [IntentManaged(Mode.Merge)]
    public class ChangeHandlerService : IChangeHandlerService
    {
        [IntentManaged(Mode.Merge)]
        public ChangeHandlerService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task AcceptChanges(List<CosmosChangeDto> changes, CancellationToken cancellationToken = default)
        {
            // TODO: Implement AcceptChanges (ChangeHandlerService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}