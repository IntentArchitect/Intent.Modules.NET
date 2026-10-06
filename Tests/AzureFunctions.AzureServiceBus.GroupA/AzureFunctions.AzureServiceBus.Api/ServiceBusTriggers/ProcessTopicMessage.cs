using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.ServiceBusTriggers.ProcessTopicMessage;
using AzureFunctions.AzureServiceBus.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Api.ServiceBusTriggers
{
    public class ProcessTopicMessage
    {
        private readonly IMediator _mediator;

        public ProcessTopicMessage(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Function("ServiceBusTriggers_ProcessTopicMessage")]
        public async Task Run(
            [ServiceBusTrigger("%AzureFunctions:ServiceBusTriggers_ProcessTopicMessage:Topic%", "%CommandTopicSubscription%")] ProcessTopicMessageCommand processTopicMessageCommand,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(processTopicMessageCommand, cancellationToken);

        }
    }
}