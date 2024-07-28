using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.OpenApiIgnoreSingle.OperationA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationAHandler : IRequestHandler<OperationA>
    {
        [IntentManaged(Mode.Merge)]
        public OperationAHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(OperationA request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (OperationAHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}