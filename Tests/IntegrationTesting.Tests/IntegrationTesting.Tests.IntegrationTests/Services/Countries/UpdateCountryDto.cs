using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Countries
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