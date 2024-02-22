using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandDtoReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandDtoReturnHandler : IRequestHandler<CommandDtoReturn, CommandResponseDto>
    {
        [IntentManaged(Mode.Merge)]
        public CommandDtoReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<CommandResponseDto> Handle(CommandDtoReturn request, CancellationToken cancellationToken)
        {
            return CommandResponseDto.Create($"{request.Input} - {DateTime.Now:s}");
        }
    }
}