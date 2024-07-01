using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetLogout
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetLogoutQueryHandler : IRequestHandler<GetLogoutQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public GetLogoutQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(GetLogoutQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetLogoutQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}