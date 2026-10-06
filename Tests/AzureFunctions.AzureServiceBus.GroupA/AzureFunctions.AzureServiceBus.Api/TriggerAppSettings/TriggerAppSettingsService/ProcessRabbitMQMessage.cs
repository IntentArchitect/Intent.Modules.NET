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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Api.TriggerAppSettings.TriggerAppSettingsService
{
    public class ProcessRabbitMQMessage
    {
        private readonly ITriggerAppSettingsService _appService;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessRabbitMQMessage(ITriggerAppSettingsService appService, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _eventBus = eventBus;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("TriggerAppSettings_TriggerAppSettingsService_ProcessRabbitMQMessage")]
        public async Task Run(
            [RabbitMQTrigger("%AzureFunctions:TriggerAppSettings_TriggerAppSettingsService_ProcessRabbitMQMessage:Queue%", ConnectionStringSetting = "RabbitMQConnection")] PayloadDto payload,
            CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.ProcessRabbitMQMessage(payload, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
                await _eventBus.FlushAllAsync(cancellationToken);

            }
        }
    }
}