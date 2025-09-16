using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using AzureFunctions.NET6.Application.Queues.CreateCustomerMessage;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.Queues.Bindings
{
    public class QueueClientBinding
    {

        public QueueClientBinding()
        {
        }

        [FunctionName("Queues_Bindings_QueueClientBinding")]
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Run(
            [QueueTrigger("queue1")] Application.Queues.CreateCustomerMessage.CreateCustomerMessage message,
            [Queue("out-queue")] QueueClient queueClient,
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }


    }
}