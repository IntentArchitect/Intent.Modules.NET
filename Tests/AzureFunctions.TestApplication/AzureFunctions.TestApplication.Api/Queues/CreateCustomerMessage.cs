using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using AzureFunctions.TestApplication.Application.Queues.CreateCustomerMessage;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class CreateCustomerMessage
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerMessage(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("CreateCustomerMessage")]
        public async Task Run([QueueTrigger("customers")] QueueMessage message, CancellationToken cancellationToken)
        {
            var createCustomerMessage = JsonConvert.DeserializeObject<Application.Queues.CreateCustomerMessage.CreateCustomerMessage>(message.Body.ToString())!;
            await _mediator.Send(createCustomerMessage, cancellationToken);
        }
    }
}