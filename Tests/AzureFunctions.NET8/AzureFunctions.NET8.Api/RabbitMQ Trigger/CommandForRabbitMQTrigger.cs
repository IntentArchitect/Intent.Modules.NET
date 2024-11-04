using System.Transactions;
using AzureFunctions.NET8.Application.RabbitMQTrigger.CommandForRabbitMQTrigger;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.RabbitMQTrigger
{
    public class CommandForRabbitMQTrigger
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CommandForRabbitMQTrigger(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("RabbitMQ Trigger_CommandForRabbitMQTrigger")]
        public async Task Run(
            [Microsoft.Azure.Functions.Worker.RabbitMQTrigger("rabbit-queue")] Application.RabbitMQTrigger.CommandForRabbitMQTrigger.CommandForRabbitMQTrigger commandForRabbitMQTrigger,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(commandForRabbitMQTrigger, cancellationToken);

        }
    }
}