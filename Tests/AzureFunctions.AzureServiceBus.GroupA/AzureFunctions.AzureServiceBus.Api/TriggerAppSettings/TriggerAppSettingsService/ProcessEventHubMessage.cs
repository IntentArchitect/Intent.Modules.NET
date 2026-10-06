using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.AzureServiceBus.Application;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Application.Interfaces.TriggerAppSettings;
using AzureFunctions.AzureServiceBus.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Api.TriggerAppSettings.TriggerAppSettingsService
{
    public class ProcessEventHubMessage
    {
        private readonly ITriggerAppSettingsService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessEventHubMessage(ITriggerAppSettingsService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("TriggerAppSettings_TriggerAppSettingsService_ProcessEventHubMessage")]
        public async Task Run(
            [EventHubTrigger("%AzureFunctions:TriggerAppSettings_TriggerAppSettingsService_ProcessEventHubMessage:EventHub%", Connection = "EventHubConnection")] List<PayloadDto> payload,
            CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.ProcessEventHubMessage(payload, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
                await _eventBus.FlushAllAsync(cancellationToken);

            }
        }
    }
}