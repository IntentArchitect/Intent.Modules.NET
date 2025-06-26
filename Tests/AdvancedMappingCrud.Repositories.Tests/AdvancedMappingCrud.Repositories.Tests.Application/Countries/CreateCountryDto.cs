using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
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