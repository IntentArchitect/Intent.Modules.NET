using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.CreateUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateUserCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}