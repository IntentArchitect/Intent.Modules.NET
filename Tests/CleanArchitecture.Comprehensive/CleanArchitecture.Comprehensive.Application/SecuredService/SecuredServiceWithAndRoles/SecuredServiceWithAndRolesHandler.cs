using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.SecuredService.SecuredServiceWithAndRoles
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SecuredServiceWithAndRolesHandler : IRequestHandler<SecuredServiceWithAndRoles>
    {
        [IntentManaged(Mode.Merge)]
        public SecuredServiceWithAndRolesHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(SecuredServiceWithAndRoles request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}