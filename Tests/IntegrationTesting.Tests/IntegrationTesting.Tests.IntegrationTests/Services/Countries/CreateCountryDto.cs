using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Countries
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