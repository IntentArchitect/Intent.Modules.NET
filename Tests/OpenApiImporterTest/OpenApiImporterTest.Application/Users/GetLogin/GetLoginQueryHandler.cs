using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetLogin
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetLoginQueryHandler : IRequestHandler<GetLoginQuery, string>
    {
        [IntentManaged(Mode.Merge)]
        public GetLoginQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(GetLoginQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetLoginQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}