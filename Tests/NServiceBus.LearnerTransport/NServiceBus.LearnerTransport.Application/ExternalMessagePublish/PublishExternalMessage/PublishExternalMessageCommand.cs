using Intent.RoslynWeaver.Attributes;
using MediatR;
using NServiceBus.LearnerTransport.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace NServiceBus.LearnerTransport.Application.ExternalMessagePublish.PublishExternalMessage
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