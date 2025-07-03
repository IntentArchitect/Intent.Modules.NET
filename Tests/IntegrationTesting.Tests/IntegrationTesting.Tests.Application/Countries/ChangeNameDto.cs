using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Countries
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