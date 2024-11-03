using System.Text.Json;
using System.Transactions;
using AzureFunctions.NET8.Application.RabbitMQTrigger.CommandForRabbitMQTrigger;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using RabbitMQ.Client.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.RabbitMQTrigger
{
    public class RabbitMQTriggerFunction
    {
        private readonly IUnitOfWork _unitOfWork;

        public RabbitMQTriggerFunction(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("RabbitMQ Trigger_RabbitMQTriggerFunction")]
        public async Task Run(
            [Microsoft.Azure.Functions.Worker.RabbitMQTrigger("rabbit-queue")] BasicDeliverEventArgs args,
            CancellationToken cancellationToken = default)
        {
            var command = JsonSerializer.Deserialize<Application.RabbitMQTrigger.CommandForRabbitMQTrigger.CommandForRabbitMQTrigger>(args.Body.ToArray());
        }
    }
}