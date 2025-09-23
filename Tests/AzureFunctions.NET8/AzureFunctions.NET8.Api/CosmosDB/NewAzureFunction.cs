using AzureFunctions.NET8.Application.CosmosDB;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.CosmosDB
{
    public class NewAzureFunction
    {

        public NewAzureFunction()
        {
        }

        [Function("CosmosDB_NewAzureFunction")]
        public async Task Run(
            [CosmosDBTrigger(databaseName: "MyDB", containerName: "Container", Connection = "Connection", CreateLeaseContainerIfNotExists = true, LeaseContainerName = "leases")] IReadOnlyCollection<CosmosChangeDto> rawCollection,
            CancellationToken cancellationToken)
        {
            if (rawCollection == null || rawCollection.Count == 0) return;
            var changes = rawCollection.ToList();
        }
    }
}