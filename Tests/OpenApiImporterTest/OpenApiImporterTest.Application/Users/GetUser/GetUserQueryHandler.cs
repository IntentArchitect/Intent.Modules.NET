using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}