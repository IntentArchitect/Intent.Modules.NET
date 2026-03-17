using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MassTransit.RequestResponse.Client.Application.TestCommands.RabbitMqTest
{
    public class RabbitMqTestCommand : IRequest, ICommand
    {
        public RabbitMqTestCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}