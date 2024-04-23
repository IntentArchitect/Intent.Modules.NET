using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetUserLogout
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetUserLogoutQueryHandler : IRequestHandler<GetUserLogoutQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserLogoutQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<int> Handle(GetUserLogoutQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}