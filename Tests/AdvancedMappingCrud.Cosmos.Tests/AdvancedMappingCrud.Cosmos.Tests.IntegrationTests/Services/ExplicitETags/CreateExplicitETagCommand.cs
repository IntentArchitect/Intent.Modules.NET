using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ExplicitETags
{
    public class CreateExplicitETagCommand
    {
        public CreateExplicitETagCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateExplicitETagCommand Create(string name)
        {
            return new CreateExplicitETagCommand
            {
                Name = name
            };
        }
    }
}