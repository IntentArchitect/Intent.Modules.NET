using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using AzureFunctions.NET6.Application.Queues.Bindings.Bind;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.Queues.Bindings
{
    public class Bind
    {
        private readonly IMediator _mediator;

        public Bind(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [FunctionName("Queues_Bindings_Bind")]
        public async Task Run(
            [QueueTrigger("in-queue")] BindCommand bindCommand,
            [Queue("out-queue")] QueueClient queueClient,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(bindCommand, cancellationToken);
            await queueClient.SendMessageAsync(JsonSerializer.Serialize(result), cancellationToken);
        }
    }
}