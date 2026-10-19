using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.AppSettings.Application;
using AzureFunctions.AppSettings.Application.Interfaces;
using AzureFunctions.AppSettings.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AppSettings.Api.TriggerAppSettingsService
{
    public class ProcessRabbitMQMessage
    {
        private readonly ITriggerAppSettingsService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessRabbitMQMessage(ITriggerAppSettingsService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("ProcessRabbitMQMessage")]
        public async Task Run(
            [RabbitMQTrigger("%AzureFunctions:ProcessRabbitMQMessage:Queue%", ConnectionStringSetting = "RabbitMQConnection")] PayloadDto payload,
            CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.ProcessRabbitMQMessage(payload, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();

            }
        }
    }
}