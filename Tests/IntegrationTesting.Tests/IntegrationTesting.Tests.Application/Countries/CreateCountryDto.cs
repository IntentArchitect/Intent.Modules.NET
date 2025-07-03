using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
{
    public class CreateCountryDto
    {
        public CreateCountryDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateCountryDto Create(string name)
        {
            return new CreateCountryDto
            {
                Name = name
            };
        }
    }
}