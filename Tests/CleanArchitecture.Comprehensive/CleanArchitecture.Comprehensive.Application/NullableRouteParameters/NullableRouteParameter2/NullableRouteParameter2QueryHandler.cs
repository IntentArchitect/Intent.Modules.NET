using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NullableRouteParameter2QueryHandler : IRequestHandler<NullableRouteParameter2Query, int>
    {
        [IntentManaged(Mode.Merge)]
        public NullableRouteParameter2QueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(NullableRouteParameter2Query request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (NullableRouteParameter2QueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}