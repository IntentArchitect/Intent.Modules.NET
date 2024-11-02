using System.Text.Json;
using System.Transactions;
using Azure.Storage.Queues.Models;
using AzureFunctions.NET8.Application.Customers;
using AzureFunctions.NET8.Application.Interfaces.Queues;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Queues.QueueService
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

        [Function("Queues_QueueService_CreateCustomerOp")]
        public async Task Run(
            [QueueTrigger("customers")] QueueMessage rawMessage,
            CancellationToken cancellationToken = default)
        {
            var dto = JsonSerializer.Deserialize<CustomerDto>(rawMessage.Body.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.CreateCustomerOp(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();

            }
        }
    }
}