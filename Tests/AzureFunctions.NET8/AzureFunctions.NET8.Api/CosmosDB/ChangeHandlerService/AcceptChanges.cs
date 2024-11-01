using System.Transactions;
using AzureFunctions.NET8.Application.CosmosDB;
using AzureFunctions.NET8.Application.Interfaces.CosmosDB;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.CosmosDB.ChangeHandlerService
{
    public class AcceptChanges
    {
        private readonly IChangeHandlerService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public AcceptChanges(IChangeHandlerService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("CosmosDB_ChangeHandlerService_AcceptChanges")]
        public async Task Run(
            [CosmosDBTrigger(databaseName: "MyDB", containerName: "Container", Connection = "Connection", CreateLeaseContainerIfNotExists = true, LeaseContainerName = "leases")] IReadOnlyCollection<CosmosChangeDto> rawCollection,
            CancellationToken cancellationToken)
        {
            if (rawCollection == null || rawCollection.Count == 0) return;
            var changes = rawCollection.ToList();

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.AcceptChanges(changes, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();

            }
        }
    }
}