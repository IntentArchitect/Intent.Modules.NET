using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries
{
    public class CreateCountryDto
    {
        public CreateCountryDto()
        {
            MaE = null!;
        }

        public string MaE { get; set; }

        public static CreateCountryDto Create(string maE)
        {
            return new CreateCountryDto
            {
                MaE = maE
            };
        }
    }
}