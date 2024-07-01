using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasMissingDeps
{
    public class HasMissingDepDto
    {
        public HasMissingDepDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MissingDepId { get; set; }

        public static HasMissingDepDto Create(Guid id, string name, Guid missingDepId)
        {
            return new HasMissingDepDto
            {
                Id = id,
                Name = name,
                MissingDepId = missingDepId
            };
        }
    }
}