using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Countries
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