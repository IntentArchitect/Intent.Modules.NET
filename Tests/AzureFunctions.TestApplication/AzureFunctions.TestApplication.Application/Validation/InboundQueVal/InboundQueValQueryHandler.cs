using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Validation.InboundQueVal
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class InboundQueValQueryHandler : IRequestHandler<InboundQueValQuery, DummyResultDto>
    {
        [IntentManaged(Mode.Merge)]
        public InboundQueValQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<DummyResultDto> Handle(InboundQueValQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}