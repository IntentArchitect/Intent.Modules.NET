using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.AzureServiceBus.Application.ServiceBusTriggers.ProcessTopicMessage
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProcessTopicMessageCommandHandler : IRequestHandler<ProcessTopicMessageCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessTopicMessageCommandHandler()
        {
        }

        /// <summary>
        /// Service Bus topic trigger on a CQRS command using a subscription and the default connection.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ProcessTopicMessageCommand request, CancellationToken cancellationToken)
        {
        }
    }
}
