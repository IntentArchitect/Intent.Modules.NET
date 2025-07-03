using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Countries
{
    public class CityDto
    {
        public CityDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid StateId { get; set; }

        public static CityDto Create(Guid id, string name, Guid stateId)
        {
            return new CityDto
            {
                Id = id,
                Name = name,
                StateId = stateId
            };
        }
    }
}