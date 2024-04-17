using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.CreateUserCreateWithList
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUserCreateWithListCommandHandler : IRequestHandler<CreateUserCreateWithListCommand, User>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserCreateWithListCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<User> Handle(CreateUserCreateWithListCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}