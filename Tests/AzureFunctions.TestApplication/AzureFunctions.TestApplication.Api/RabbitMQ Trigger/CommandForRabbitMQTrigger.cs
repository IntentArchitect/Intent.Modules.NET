using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunctions.TestApplication.Application.RabbitMQTrigger.CommandForRabbitMQTrigger;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.RabbitMQTrigger
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

        [FunctionName("RabbitMQ Trigger_CommandForRabbitMQTrigger")]
        public async Task Run(
            [Microsoft.Azure.WebJobs.RabbitMQTrigger("rabbit-queue")] Application.RabbitMQTrigger.CommandForRabbitMQTrigger.CommandForRabbitMQTrigger commandForRabbitMQTrigger,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(commandForRabbitMQTrigger, cancellationToken);

        }
    }
}