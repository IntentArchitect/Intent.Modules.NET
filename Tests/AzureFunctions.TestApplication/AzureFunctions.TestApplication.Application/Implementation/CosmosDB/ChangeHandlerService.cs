using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.CosmosDB;
using AzureFunctions.TestApplication.Application.Interfaces.CosmosDB;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation.CosmosDB
{
    [IntentManaged(Mode.Merge)]
    public class ChangeHandlerService : IChangeHandlerService
    {
        [IntentManaged(Mode.Merge)]
        public ChangeHandlerService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task AcceptChanges(List<CosmosChangeDto> changes, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}