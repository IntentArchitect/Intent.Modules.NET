using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;
using RabbitMQ.Client.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class RabbitMQTriggerFunction
    {
        private readonly IUnitOfWork _unitOfWork;

        public RabbitMQTriggerFunction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("RabbitMQTriggerFunction")]
        public async Task Run(
            [RabbitMQTrigger("rabbit-queue")] BasicDeliverEventArgs args,
            CancellationToken cancellationToken)
        {
            var command = JsonSerializer.Deserialize<CommandForRabbitMQTrigger>(args.Body.ToArray());
        }
    }
}