using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;
using OpenApiImporterTest.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetPet
{
    [Authorize]
    public class GetPetQuery : IRequest<Pet>, IQuery
    {
        public GetPetQuery(int petId)
        {
            PetId = petId;
        }

        public int PetId { get; set; }
    }
}