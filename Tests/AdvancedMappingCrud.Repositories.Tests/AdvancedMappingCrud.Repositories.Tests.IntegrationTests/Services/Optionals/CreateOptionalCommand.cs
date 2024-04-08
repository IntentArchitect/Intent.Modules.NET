using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Optionals
{
    public class CreateOptionalCommand
    {
        public CreateOptionalCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateOptionalCommand Create(string name)
        {
            return new CreateOptionalCommand
            {
                Name = name
            };
        }
    }
}