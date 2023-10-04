using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.CosmosDB;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Interfaces.CosmosDB
{
    public interface IChangeHandlerService : IDisposable
    {
        Task AcceptChanges(List<CosmosChangeDto> changes, CancellationToken cancellationToken = default);
    }
}