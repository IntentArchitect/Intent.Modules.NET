using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.SimpleOdata
{
    public class CreateSimpleOdataCommand
    {
        public CreateSimpleOdataCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }

        public static CreateSimpleOdataCommand Create(string name, string surname)
        {
            return new CreateSimpleOdataCommand
            {
                Name = name,
                Surname = surname
            };
        }
    }
}