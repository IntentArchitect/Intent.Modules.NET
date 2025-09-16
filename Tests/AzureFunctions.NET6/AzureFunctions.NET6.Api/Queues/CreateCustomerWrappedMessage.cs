using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using AzureFunctions.NET6.Application.Queues.CreateCustomerWrappedMessage;
using AzureFunctions.NET6.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET6.Api.Queues
{
    public class CreateCustomerWrappedMessage
    {
        private readonly IMediator _mediator;

        public CreateCustomerWrappedMessage(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [FunctionName("Queues_CreateCustomerWrappedMessage")]
        public async Task Run([QueueTrigger("customers")] QueueMessage rawMessage, CancellationToken cancellationToken)
        {
            var createCustomerWrappedMessage = JsonSerializer.Deserialize<Application.Queues.CreateCustomerWrappedMessage.CreateCustomerWrappedMessage>(rawMessage.Body.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            await _mediator.Send(createCustomerWrappedMessage, cancellationToken);

        }
    }
}