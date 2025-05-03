using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.NamingOverrides.SendFromEventHandler
{
    public class SendFromEventHandlerCommand : IRequest, ICommand
    {
        public SendFromEventHandlerCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}