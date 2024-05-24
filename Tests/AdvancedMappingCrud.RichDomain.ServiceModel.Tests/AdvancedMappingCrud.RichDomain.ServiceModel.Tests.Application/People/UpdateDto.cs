using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class UpdateDto
    {
        public UpdateDto()
        {
            Details = null!;
        }

        public Guid Id { get; set; }
        public UpdateUpdatePersonDetailsDto Details { get; set; }

        public static UpdateDto Create(Guid id, UpdateUpdatePersonDetailsDto details)
        {
            return new UpdateDto
            {
                Id = id,
                Details = details
            };
        }
    }
}