using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetUserLogin
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetUserLoginQueryHandler : IRequestHandler<GetUserLoginQuery, string>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserLoginQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(GetUserLoginQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}