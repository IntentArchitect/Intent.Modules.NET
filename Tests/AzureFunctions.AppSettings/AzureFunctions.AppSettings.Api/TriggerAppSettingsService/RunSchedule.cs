using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.AppSettings.Application.Interfaces;
using AzureFunctions.AppSettings.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using TimerInfo = Microsoft.Azure.Functions.Worker.TimerInfo;
using TimerTriggerAttribute = Microsoft.Azure.Functions.Worker.TimerTriggerAttribute;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AppSettings.Api.TriggerAppSettingsService
{
    public class RunSchedule
    {
        private readonly ITriggerAppSettingsService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public RunSchedule(ITriggerAppSettingsService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("RunSchedule")]
        public async Task Run(
            [TimerTrigger("%AzureFunctions:RunSchedule:Schedule%")] TimerInfo timerInfo,
            CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.RunSchedule(cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();

            }
        }
    }
}