using System.Text.Json;
using System.Transactions;
using Azure.Storage.Queues.Models;
using AzureFunctions.NET8.Application.Queues.CreateCustomerWrappedMessage;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Queues
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

        [Function("Queues_CreateCustomerWrappedMessage")]
        public async Task Run([QueueTrigger("customers")] QueueMessage rawMessage, CancellationToken cancellationToken)
        {
            var createCustomerWrappedMessage = JsonSerializer.Deserialize<Application.Queues.CreateCustomerWrappedMessage.CreateCustomerWrappedMessage>(rawMessage.Body.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            await _mediator.Send(createCustomerWrappedMessage, cancellationToken);

        }
    }
}