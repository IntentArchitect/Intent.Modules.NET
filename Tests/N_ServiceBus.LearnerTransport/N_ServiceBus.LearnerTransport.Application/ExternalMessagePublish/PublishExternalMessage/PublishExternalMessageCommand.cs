using Intent.RoslynWeaver.Attributes;
using MediatR;
using N_ServiceBus.LearnerTransport.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.Application.ExternalMessagePublish.PublishExternalMessage
{
    public class PublishExternalMessageCommand : IRequest, ICommand
    {
        public PublishExternalMessageCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}