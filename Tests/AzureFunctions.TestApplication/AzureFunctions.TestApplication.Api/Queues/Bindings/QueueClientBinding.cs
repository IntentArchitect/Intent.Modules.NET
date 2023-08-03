using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues;
using AzureFunctions.TestApplication.Application.Queues.CreateCustomerMessage;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class QueueClientBinding
    {
        private readonly IUnitOfWork _unitOfWork;

        public QueueClientBinding(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("QueueClientBinding")]
        public async Task Run(
            [QueueTrigger("queue1")] CreateCustomerMessage message,
            [Queue("out-queue")] QueueClient queueClient,
            CancellationToken cancellationToken)
        {
        }
    }
}