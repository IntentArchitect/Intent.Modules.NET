using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies
{
    public class CreateBasicOrderByCommand
    {
        public CreateBasicOrderByCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }

        public static CreateBasicOrderByCommand Create(string name, string surname)
        {
            return new CreateBasicOrderByCommand
            {
                Name = name,
                Surname = surname
            };
        }
    }
}