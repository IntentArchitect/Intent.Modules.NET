using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.UpdateUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateUserCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateUserCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}