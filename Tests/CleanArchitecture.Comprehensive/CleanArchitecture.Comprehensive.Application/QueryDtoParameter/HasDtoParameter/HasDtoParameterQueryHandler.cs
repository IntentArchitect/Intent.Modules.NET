using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.QueryDtoParameter.HasDtoParameter
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class HasDtoParameterQueryHandler : IRequestHandler<HasDtoParameterQuery, int>
    {
        [IntentManaged(Mode.Ignore)]
        public HasDtoParameterQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(HasDtoParameterQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}