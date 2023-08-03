using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using AzureFunctions.TestApplication.Application.Customers;
using AzureFunctions.TestApplication.Application.Interfaces.Queues;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class CreateCustomerOpWrapped
    {
        private readonly IQueueService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerOpWrapped(IQueueService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("CreateCustomerOpWrapped")]
        public async Task Run([QueueTrigger("customers")] QueueMessage message, CancellationToken cancellationToken)
        {
            var dto = JsonConvert.DeserializeObject<CustomerDto>(message.Body.ToString())!;
            await _appService.CreateCustomerOpWrapped(dto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }
    }
}