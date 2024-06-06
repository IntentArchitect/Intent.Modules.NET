using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyCommandRequest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyCommandRequestHandler : IRequestHandler<MyCommandRequest>
    {
        [IntentManaged(Mode.Merge)]
        public MyCommandRequestHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(MyCommandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}