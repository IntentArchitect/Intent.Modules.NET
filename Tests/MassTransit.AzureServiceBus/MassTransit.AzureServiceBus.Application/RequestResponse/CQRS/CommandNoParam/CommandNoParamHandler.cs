using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandNoParam
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandNoParamHandler : IRequestHandler<CommandNoParam>
    {
        [IntentManaged(Mode.Merge)]
        public CommandNoParamHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CommandNoParam request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}