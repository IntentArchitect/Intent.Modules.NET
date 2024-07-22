using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;
using OpenApiImporterTest.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.DeletePet
{
    [Authorize]
    public class DeletePetCommand : IRequest, ICommand
    {
        public DeletePetCommand(string api_key, int petId)
        {
            Api_key = api_key;
            PetId = petId;
        }

        public string Api_key { get; set; }
        public int PetId { get; set; }
    }
}