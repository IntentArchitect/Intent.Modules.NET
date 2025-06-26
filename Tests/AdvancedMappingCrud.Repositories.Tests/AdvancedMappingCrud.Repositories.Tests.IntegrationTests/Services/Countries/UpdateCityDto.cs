using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries
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