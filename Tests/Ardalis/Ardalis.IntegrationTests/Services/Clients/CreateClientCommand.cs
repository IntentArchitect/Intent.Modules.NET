using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace Ardalis.IntegrationTests.Services.Clients
{
    public class CreateClientCommand
    {
        public CreateClientCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateClientCommand Create(string name)
        {
            return new CreateClientCommand
            {
                Name = name
            };
        }
    }
}