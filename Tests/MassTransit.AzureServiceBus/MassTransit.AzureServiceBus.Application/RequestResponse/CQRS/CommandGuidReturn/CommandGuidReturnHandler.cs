using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandGuidReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandGuidReturnHandler : IRequestHandler<CommandGuidReturn, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public CommandGuidReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CommandGuidReturn request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}