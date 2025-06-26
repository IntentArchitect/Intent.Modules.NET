using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
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