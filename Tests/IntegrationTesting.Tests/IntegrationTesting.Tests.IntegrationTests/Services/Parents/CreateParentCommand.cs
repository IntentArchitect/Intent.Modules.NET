using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Parents
{
    public class CreateParentCommand
    {
        public CreateParentCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateParentCommand Create(string name)
        {
            return new CreateParentCommand
            {
                Name = name
            };
        }
    }
}