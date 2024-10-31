using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.CosmosDB;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Interfaces.CosmosDB
{
    public interface IChangeHandlerService
    {
        Task AcceptChanges(List<CosmosChangeDto> changes, CancellationToken cancellationToken = default);
    }
}