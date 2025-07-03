using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    public class UpdateCountryDto
    {
        public UpdateCountryDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateCountryDto Create(string name)
        {
            return new UpdateCountryDto
            {
                Name = name
            };
        }
    }
}