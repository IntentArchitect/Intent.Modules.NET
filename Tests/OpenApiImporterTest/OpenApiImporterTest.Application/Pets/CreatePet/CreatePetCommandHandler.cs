using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.CreatePet
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePetCommandHandler : IRequestHandler<CreatePetCommand, Pet>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePetCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Pet> Handle(CreatePetCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}