using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    public class UpdateCityDto
    {
        public UpdateCityDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid StateId { get; set; }

        public static UpdateCityDto Create(string name, Guid stateId)
        {
            return new UpdateCityDto
            {
                Name = name,
                StateId = stateId
            };
        }
    }
}