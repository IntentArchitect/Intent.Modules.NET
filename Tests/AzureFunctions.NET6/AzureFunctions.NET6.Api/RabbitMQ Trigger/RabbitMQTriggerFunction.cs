using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.NET6.Application.RabbitMQTrigger.CommandForRabbitMQTrigger;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;
using RabbitMQ.Client.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.RabbitMQTrigger
{
    public class RabbitMQTriggerFunction
    {
        private readonly IUnitOfWork _unitOfWork;

        public RabbitMQTriggerFunction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("RabbitMQ Trigger_RabbitMQTriggerFunction")]
        public async Task Run(
            [Microsoft.Azure.WebJobs.RabbitMQTrigger("rabbit-queue")] BasicDeliverEventArgs args,
            CancellationToken cancellationToken = default)
        {
            var command = JsonSerializer.Deserialize<Application.RabbitMQTrigger.CommandForRabbitMQTrigger.CommandForRabbitMQTrigger>(args.Body.ToArray());
        }
    }
}