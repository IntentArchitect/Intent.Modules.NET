using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs
{
    public class CreateEntitiesCommandentitiesDto
    {
        public CreateEntitiesCommandentitiesDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CreateEntitiesCommandentitiesDto Create(Guid id, string name)
        {
            return new CreateEntitiesCommandentitiesDto
            {
                Id = id,
                Name = name
            };
        }
    }
}