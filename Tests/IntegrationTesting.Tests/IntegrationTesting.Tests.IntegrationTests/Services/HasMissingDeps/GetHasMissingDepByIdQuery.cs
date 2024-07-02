using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.HasMissingDeps
{
    public class GetHasMissingDepByIdQuery
    {
        public Guid Id { get; set; }

        public static GetHasMissingDepByIdQuery Create(Guid id)
        {
            return new GetHasMissingDepByIdQuery
            {
                Id = id
            };
        }
    }
}