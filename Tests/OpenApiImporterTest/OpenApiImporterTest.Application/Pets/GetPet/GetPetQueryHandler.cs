using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetPet
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPetQueryHandler : IRequestHandler<GetPetQuery, Pet>
    {
        [IntentManaged(Mode.Merge)]
        public GetPetQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Pet> Handle(GetPetQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}