using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues;
using AzureFunctions.NET8.Application.Customers;
using AzureFunctions.NET8.Application.Queues.Bindings.Bind;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Queues.Bindings
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

        [Function("Queues_Bindings_Bind")]
        [QueueOutput("out-queue")]
        public async Task<CustomerDto> Run(
            [QueueTrigger("in-queue")] BindCommand bindCommand,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(bindCommand, cancellationToken);
            return result;
        }
    }
}