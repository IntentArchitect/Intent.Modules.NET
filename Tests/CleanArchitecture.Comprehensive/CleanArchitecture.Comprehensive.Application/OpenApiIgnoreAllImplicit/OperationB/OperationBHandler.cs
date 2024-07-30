using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.OpenApiIgnoreAllImplicit.OperationB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OperationBHandler : IRequestHandler<OperationB>
    {
        [IntentManaged(Mode.Merge)]
        public OperationBHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(OperationB request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (OperationBHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}