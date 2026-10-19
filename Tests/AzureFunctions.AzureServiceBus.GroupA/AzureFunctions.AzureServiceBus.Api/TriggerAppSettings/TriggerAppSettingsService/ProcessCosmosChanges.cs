using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.AzureServiceBus.Application;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Application.Interfaces.TriggerAppSettings;
using AzureFunctions.AzureServiceBus.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Api.TriggerAppSettings.TriggerAppSettingsService
{
    public class ProcessCosmosChanges
    {
        private readonly ITriggerAppSettingsService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessCosmosChanges(ITriggerAppSettingsService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("TriggerAppSettings_TriggerAppSettingsService_ProcessCosmosChanges")]
        public async Task Run(
            [CosmosDBTrigger(databaseName: "%AzureFunctions:TriggerAppSettings_TriggerAppSettingsService_ProcessCosmosChanges:Database%", containerName: "%AzureFunctions:TriggerAppSettings_TriggerAppSettingsService_ProcessCosmosChanges:Container%", Connection = "CosmosConnection", CreateLeaseContainerIfNotExists = false, LeaseContainerName = "leases", LeaseContainerPrefix = "orders-", LeaseDatabaseName = "leases-db")] IReadOnlyCollection<PayloadDto> rawCollection,
            CancellationToken cancellationToken)
        {
            if (rawCollection == null || rawCollection.Count == 0) return;

            foreach (var payload in rawCollection)
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.ProcessCosmosChanges(payload, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                    await _eventBus.FlushAllAsync(cancellationToken);

                }
            }
        }
    }
}