using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Validation.ResultValidations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ResultValidationsQueryHandler : IRequestHandler<ResultValidationsQuery, ValidatedResultDto>
    {
        [IntentManaged(Mode.Ignore)]
        public ResultValidationsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<ValidatedResultDto> Handle(ResultValidationsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}