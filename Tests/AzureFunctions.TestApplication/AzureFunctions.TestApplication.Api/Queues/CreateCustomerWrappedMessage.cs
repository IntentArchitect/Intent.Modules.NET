using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues.Models;
using AzureFunctions.TestApplication.Application.Queues.CreateCustomerWrappedMessage;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.TestApplication.Api.Queues
{
    public class CreateCustomerWrappedMessage
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerWrappedMessage(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("Queues_CreateCustomerWrappedMessage")]
        public async Task Run([QueueTrigger("customers")] QueueMessage rawMessage, CancellationToken cancellationToken)
        {
            var createCustomerWrappedMessage = JsonSerializer.Deserialize<Application.Queues.CreateCustomerWrappedMessage.CreateCustomerWrappedMessage>(rawMessage.Body.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            await _mediator.Send(createCustomerWrappedMessage, cancellationToken);

        }
    }
}