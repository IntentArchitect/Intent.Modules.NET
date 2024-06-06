using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyRequest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MyRequestCommandHandler : IRequestHandler<MyRequestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public MyRequestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(MyRequestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}