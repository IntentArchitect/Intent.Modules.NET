using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasMissingDeps
{
    public class DeleteHasMissingDepCommand
    {
        public Guid Id { get; set; }

        public static DeleteHasMissingDepCommand Create(Guid id)
        {
            return new DeleteHasMissingDepCommand
            {
                Id = id
            };
        }
    }
}