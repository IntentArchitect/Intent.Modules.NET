using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.CosmosDB.CosmosChange;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.CosmosDB
{
    public class CosmosChange
    {
        private readonly IMediator _mediator;

        public CosmosChange(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [FunctionName("CosmosDB_CosmosChange")]
        public async Task Run(
            [CosmosDBTrigger(databaseName: "DBName", containerName: "Container", Connection = "Connection", CreateLeaseContainerIfNotExists = true, LeaseContainerName = "leases")] IReadOnlyCollection<CosmosChangeCommand> rawCollection,
            CancellationToken cancellationToken)
        {
            if (rawCollection == null || rawCollection.Count == 0) return;

            foreach (var cosmosChangeCommand in rawCollection)
            {
                await _mediator.Send(cosmosChangeCommand, cancellationToken);

            }
        }
    }
}