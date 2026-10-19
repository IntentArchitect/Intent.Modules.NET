using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Application.ServiceBusTriggers.ProcessQueueMessage
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProcessQueueMessageCommandHandler : IRequestHandler<ProcessQueueMessageCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessQueueMessageCommandHandler()
        {
        }

        /// <summary>
        /// Service Bus queue trigger on a CQRS command (no subscription).
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ProcessQueueMessageCommand request, CancellationToken cancellationToken)
        {
        }
    }
}
