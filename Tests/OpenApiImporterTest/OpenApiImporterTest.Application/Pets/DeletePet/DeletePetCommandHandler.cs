using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.DeletePet
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeletePetCommandHandler : IRequestHandler<DeletePetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeletePetCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DeletePetCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}