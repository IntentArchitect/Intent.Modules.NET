using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries
{
    public class CountryDto
    {
        public CountryDto()
        {
            MaE = null!;
        }

        public Guid Id { get; set; }
        public string MaE { get; set; }

        public static CountryDto Create(Guid id, string maE)
        {
            return new CountryDto
            {
                Id = id,
                MaE = maE
            };
        }
    }
}