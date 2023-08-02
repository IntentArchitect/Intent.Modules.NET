using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Customers;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.WebJobs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class MessageBinding
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageBinding(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("MessageBinding")]
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void Run(
            [QueueTrigger("queue1")] CustomerDto message,
            [Queue("out-queue")] out CustomerDto output,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your custom logic here...");
        }
    }
}