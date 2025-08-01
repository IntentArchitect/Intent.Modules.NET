using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Ignores.CommandWithIgnoreInApi
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandWithIgnoreInApiHandler : IRequestHandler<CommandWithIgnoreInApi>
    {
        [IntentManaged(Mode.Merge)]
        public CommandWithIgnoreInApiHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CommandWithIgnoreInApi request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CommandWithIgnoreInApiHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}