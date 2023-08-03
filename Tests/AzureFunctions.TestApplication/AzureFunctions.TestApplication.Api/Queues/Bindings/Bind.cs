using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues;
using AzureFunctions.TestApplication.Application.Queues.Bindings.Bind;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
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

        [FunctionName("Bind")]
        public async Task Run(
            [QueueTrigger("in-queue")] BindCommand bindCommand,
            [Queue("out-queue")] QueueClient queueClient,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(bindCommand, cancellationToken);
            await queueClient.SendMessageAsync(JsonConvert.SerializeObject(result), cancellationToken);
        }
    }
}