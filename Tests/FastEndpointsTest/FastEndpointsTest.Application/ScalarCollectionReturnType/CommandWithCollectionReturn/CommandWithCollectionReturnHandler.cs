using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.ScalarCollectionReturnType.CommandWithCollectionReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandWithCollectionReturnHandler : IRequestHandler<CommandWithCollectionReturn, List<string>>
    {
        [IntentManaged(Mode.Merge)]
        public CommandWithCollectionReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<string>> Handle(CommandWithCollectionReturn request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CommandWithCollectionReturnHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}