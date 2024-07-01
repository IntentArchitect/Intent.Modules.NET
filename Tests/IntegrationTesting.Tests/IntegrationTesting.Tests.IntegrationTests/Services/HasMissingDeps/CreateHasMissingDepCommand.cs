using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasMissingDeps
{
    public class CreateHasMissingDepCommand
    {
        public CreateHasMissingDepCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid MissingDepId { get; set; }

        public static CreateHasMissingDepCommand Create(string name, Guid missingDepId)
        {
            return new CreateHasMissingDepCommand
            {
                Name = name,
                MissingDepId = missingDepId
            };
        }
    }
}