using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.DeleteUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteUserCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}