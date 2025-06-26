using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries
{
    public class UpdateCountryDto
    {
        public UpdateCountryDto()
        {
            MaE = null!;
        }

        public string MaE { get; set; }

        public static UpdateCountryDto Create(string maE)
        {
            return new UpdateCountryDto
            {
                MaE = maE
            };
        }
    }
}