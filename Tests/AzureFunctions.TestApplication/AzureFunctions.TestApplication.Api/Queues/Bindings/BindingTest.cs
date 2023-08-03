using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using AzureFunctions.TestApplication.Application.Customers;
using AzureFunctions.TestApplication.Application.Interfaces.Queues.Bindings;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class BindingTest
    {
        private readonly IBindingService _appService;
        private readonly IUnitOfWork _unitOfWork;

        public BindingTest(IBindingService appService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("BindingTest")]
        public async Task Run(
            [QueueTrigger("in-queue")] CustomerDto dto,
            [Queue("out-queue")] QueueClient queueClient,
            CancellationToken cancellationToken)
        {
            var result = await _appService.BindingTest(dto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await queueClient.SendMessageAsync(JsonConvert.SerializeObject(result), cancellationToken);
        }
    }
}