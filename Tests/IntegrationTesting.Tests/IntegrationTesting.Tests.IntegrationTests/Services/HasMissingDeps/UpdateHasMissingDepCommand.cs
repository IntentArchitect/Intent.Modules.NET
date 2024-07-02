using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasMissingDeps
{
    public class UpdateHasMissingDepCommand
    {
        public UpdateHasMissingDepCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid MissingDepId { get; set; }
        public Guid Id { get; set; }

        public static UpdateHasMissingDepCommand Create(string name, Guid missingDepId, Guid id)
        {
            return new UpdateHasMissingDepCommand
            {
                Name = name,
                MissingDepId = missingDepId,
                Id = id
            };
        }
    }
}