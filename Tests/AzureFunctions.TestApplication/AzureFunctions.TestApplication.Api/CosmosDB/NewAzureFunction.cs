using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.TestApplication.Application.CosmosDB;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.CosmosDB
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