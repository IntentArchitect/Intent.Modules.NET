using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues;
using AzureFunctions.NET8.Application.Customers;
using AzureFunctions.NET8.Application.Queues.CreateCustomerMessage;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Queues.Bindings
{
    public class QueueClientBinding
    {
        private readonly IUnitOfWork _unitOfWork;

        public QueueClientBinding(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("Queues_Bindings_QueueClientBinding")]
        public async Task<CustomerDto> Run(
            [QueueTrigger("queue1")] Application.Queues.CreateCustomerMessage.CreateCustomerMessage message,
            CancellationToken cancellationToken)
        {
            return default;
        }
    }
}