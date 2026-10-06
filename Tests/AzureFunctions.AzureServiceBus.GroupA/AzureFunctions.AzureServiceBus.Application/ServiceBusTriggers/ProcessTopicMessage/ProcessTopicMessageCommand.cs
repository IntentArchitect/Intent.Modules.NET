using AzureFunctions.AzureServiceBus.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.ServiceBusTriggers.ProcessTopicMessage
{
    /// <summary>
    /// Service Bus topic trigger on a CQRS command using a subscription and the default connection.
    /// </summary>
    public class ProcessTopicMessageCommand : IRequest, ICommand
    {
        public ProcessTopicMessageCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}