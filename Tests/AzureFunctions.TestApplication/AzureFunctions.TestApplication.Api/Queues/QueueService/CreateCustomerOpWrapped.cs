using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues.Models;
using AzureFunctions.TestApplication.Application.Customers;
using AzureFunctions.TestApplication.Application.Interfaces.Queues;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.Queues.QueueService
{
    public class CreateCustomerOpWrapped
    {
        private readonly IQueueService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerOpWrapped(IQueueService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("Queues_QueueService_CreateCustomerOpWrapped")]
        public async Task Run([QueueTrigger("customers")] QueueMessage rawMessage, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<CustomerDto>(rawMessage.Body.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.CreateCustomerOpWrapped(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();

            }
        }
    }
}