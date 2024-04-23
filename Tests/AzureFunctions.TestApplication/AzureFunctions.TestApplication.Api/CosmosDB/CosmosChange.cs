using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.TestApplication.Application.CosmosDB.CosmosChange;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.CosmosDB
{
    public class CosmosChange
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CosmosChange(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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