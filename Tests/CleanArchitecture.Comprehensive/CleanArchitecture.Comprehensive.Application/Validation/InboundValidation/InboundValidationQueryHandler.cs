using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Validation.InboundValidation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class InboundValidationQueryHandler : IRequestHandler<InboundValidationQuery, DummyResultDto>
    {
        [IntentManaged(Mode.Merge)]
        public InboundValidationQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<DummyResultDto> Handle(InboundValidationQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}