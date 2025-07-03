using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    public class CreateCityDto
    {
        public CreateCityDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid StateId { get; set; }

        public static CreateCityDto Create(string name, Guid stateId)
        {
            return new CreateCityDto
            {
                Name = name,
                StateId = stateId
            };
        }
    }
}