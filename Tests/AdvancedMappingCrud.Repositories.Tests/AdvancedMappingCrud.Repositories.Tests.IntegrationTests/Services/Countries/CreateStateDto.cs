using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries
{
    public class CreateStateDto
    {
        public CreateStateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid CountryId { get; set; }

        public static CreateStateDto Create(string name, Guid countryId)
        {
            return new CreateStateDto
            {
                Name = name,
                CountryId = countryId
            };
        }
    }
}