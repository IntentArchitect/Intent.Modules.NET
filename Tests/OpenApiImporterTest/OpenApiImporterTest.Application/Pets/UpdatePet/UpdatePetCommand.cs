using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;
using OpenApiImporterTest.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.UpdatePet
{
    [Authorize]
    public class UpdatePetCommand : IRequest<Pet>, ICommand
    {
        public UpdatePetCommand(int? id,
            string name,
            Category? category,
            List<string> photoUrls,
            List<Tag>? tags,
            StatusType? status)
        {
            Id = id;
            Name = name;
            Category = category;
            PhotoUrls = photoUrls;
            Tags = tags;
            Status = status;
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public Category? Category { get; set; }
        public List<string> PhotoUrls { get; set; }
        public List<Tag>? Tags { get; set; }
        public StatusType? Status { get; set; }
    }
}