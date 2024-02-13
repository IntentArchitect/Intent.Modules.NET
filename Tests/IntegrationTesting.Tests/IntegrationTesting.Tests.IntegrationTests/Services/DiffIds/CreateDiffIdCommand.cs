using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DiffIds
{
    public class CreateDiffIdCommand
    {
        public CreateDiffIdCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateDiffIdCommand Create(string name)
        {
            return new CreateDiffIdCommand
            {
                Name = name
            };
        }
    }
}