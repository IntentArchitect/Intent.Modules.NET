using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ScalarCollectionReturnType.CommandWithCollectionReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandWithCollectionReturnHandler : IRequestHandler<CommandWithCollectionReturn, List<string>>
    {
        [IntentManaged(Mode.Merge)]
        public CommandWithCollectionReturnHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<string>> Handle(CommandWithCollectionReturn request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}