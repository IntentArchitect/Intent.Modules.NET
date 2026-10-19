using AzureFunctions.AzureServiceBus.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.ServiceBusTriggers.ProcessQueueMessage
{
    /// <summary>
    /// Service Bus queue trigger on a CQRS command (no subscription).
    /// </summary>
    public class ProcessQueueMessageCommand : IRequest, ICommand
    {
        public ProcessQueueMessageCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}