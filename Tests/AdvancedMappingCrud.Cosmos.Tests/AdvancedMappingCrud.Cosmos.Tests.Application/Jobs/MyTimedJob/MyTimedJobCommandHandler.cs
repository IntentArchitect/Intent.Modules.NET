using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Jobs.MyTimedJob
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyTimedJobCommandHandler : IRequestHandler<MyTimedJobCommand>
    {
        [IntentManaged(Mode.Merge)]
        public MyTimedJobCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(MyTimedJobCommand request, CancellationToken cancellationToken)
        {
            // NO-OP
        }
    }
}