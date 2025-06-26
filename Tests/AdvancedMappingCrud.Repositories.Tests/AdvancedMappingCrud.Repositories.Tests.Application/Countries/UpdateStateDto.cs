using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
{
    public class UpdateStateDto
    {
        public UpdateStateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid CountryId { get; set; }

        public static UpdateStateDto Create(string name, Guid countryId)
        {
            return new UpdateStateDto
            {
                Name = name,
                CountryId = countryId
            };
        }
    }
}