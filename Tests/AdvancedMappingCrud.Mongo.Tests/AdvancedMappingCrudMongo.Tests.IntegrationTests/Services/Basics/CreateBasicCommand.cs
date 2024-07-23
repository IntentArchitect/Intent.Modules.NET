using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Basics
{
    public class CreateBasicCommand
    {
        public CreateBasicCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateBasicCommand Create(string name)
        {
            return new CreateBasicCommand
            {
                Name = name
            };
        }
    }
}