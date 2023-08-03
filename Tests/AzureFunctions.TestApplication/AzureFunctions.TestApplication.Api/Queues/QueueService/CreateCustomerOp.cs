using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues.Models;
using AzureFunctions.TestApplication.Application.Customers;
using AzureFunctions.TestApplication.Application.Interfaces.Queues;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class CreateCustomerOp
    {
        private readonly IQueueService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerOp(IQueueService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("CreateCustomerOp")]
        public async Task Run([QueueTrigger("customers")] QueueMessage message, CancellationToken cancellationToken)
        {
            var dto = JsonConvert.DeserializeObject<CustomerDto>(message.Body.ToString())!;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.CreateCustomerOp(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
                return;
            }
        }
    }
}