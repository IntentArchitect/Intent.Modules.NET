using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.RabbitMQTrigger.CommandForRabbitMQTrigger;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.RabbitMQTrigger
{
    public class CommandForRabbitMQTrigger
    {
        private readonly IMediator _mediator;

        public CommandForRabbitMQTrigger(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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