using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues;
using AzureFunctions.TestApplication.Application.Queues.Bindings.Bind;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.Queues.Bindings
{
    public class Bind
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public Bind(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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