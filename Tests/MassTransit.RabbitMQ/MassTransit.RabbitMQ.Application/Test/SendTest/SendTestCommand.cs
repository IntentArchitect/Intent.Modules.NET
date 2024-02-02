using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.Test.SendTest
{
    public class SendTestCommand : IRequest, ICommand
    {
        public SendTestCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}