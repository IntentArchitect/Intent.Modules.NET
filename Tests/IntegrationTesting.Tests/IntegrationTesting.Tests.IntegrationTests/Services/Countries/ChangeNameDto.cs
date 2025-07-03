using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Countries
{
    public class ChangeNameDto
    {
        public ChangeNameDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static ChangeNameDto Create(string name)
        {
            return new ChangeNameDto
            {
                Name = name
            };
        }
    }
}