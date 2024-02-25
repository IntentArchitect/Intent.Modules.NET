using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MassTransit.RabbitMQ.Application.RequestResponse.CQRS.CommandDtoReturn
{
    public class CommandDtoReturn : IRequest<CommandResponseDto>, ICommand
    {
        public CommandDtoReturn(string input)
        {
            Input = input;
        }

        public string Input { get; set; }
    }
}