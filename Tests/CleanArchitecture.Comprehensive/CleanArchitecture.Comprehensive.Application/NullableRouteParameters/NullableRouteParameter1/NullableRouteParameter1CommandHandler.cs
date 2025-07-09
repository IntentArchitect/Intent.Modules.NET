using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.NullableRouteParameters.NullableRouteParameter1
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NullableRouteParameter1CommandHandler : IRequestHandler<NullableRouteParameter1Command>
    {
        [IntentManaged(Mode.Merge)]
        public NullableRouteParameter1CommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(NullableRouteParameter1Command request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (NullableRouteParameter1CommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}