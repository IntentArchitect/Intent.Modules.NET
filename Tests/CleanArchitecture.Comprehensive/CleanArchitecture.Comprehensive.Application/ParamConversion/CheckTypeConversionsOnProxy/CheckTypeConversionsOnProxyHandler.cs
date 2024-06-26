using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ParamConversion.CheckTypeConversionsOnProxy
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CheckTypeConversionsOnProxyHandler : IRequestHandler<CheckTypeConversionsOnProxy, bool>
    {
        [IntentManaged(Mode.Merge)]
        public CheckTypeConversionsOnProxyHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<bool> Handle(CheckTypeConversionsOnProxy request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CheckTypeConversionsOnProxyHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}