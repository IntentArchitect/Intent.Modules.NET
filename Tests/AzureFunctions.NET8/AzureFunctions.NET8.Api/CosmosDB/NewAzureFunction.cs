using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.NET8.Application.CosmosDB;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.CosmosDB
{
    public class NewAzureFunction
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewAzureFunction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("CosmosDB_NewAzureFunction")]
        public async Task Run(
            [CosmosDBTrigger(databaseName: "MyDB", containerName: "Container", Connection = "Connection", CreateLeaseContainerIfNotExists = true, LeaseContainerName = "leases")] IReadOnlyCollection<CosmosChangeDto> rawCollection,
            CancellationToken cancellationToken)
        {
            if (rawCollection == null || rawCollection.Count == 0) return;
            var changes = rawCollection.ToList();
        }
    }
}