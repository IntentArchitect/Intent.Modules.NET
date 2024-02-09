using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.PartialCruds
{
    public class CreatePartialCrudCommand
    {
        public CreatePartialCrudCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreatePartialCrudCommand Create(string name)
        {
            return new CreatePartialCrudCommand
            {
                Name = name
            };
        }
    }
}