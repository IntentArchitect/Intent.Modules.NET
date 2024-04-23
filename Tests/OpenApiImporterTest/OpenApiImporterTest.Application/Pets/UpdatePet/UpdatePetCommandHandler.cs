using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.UpdatePet
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand, Pet>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePetCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Pet> Handle(UpdatePetCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}