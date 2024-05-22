using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace ValueObjects.Class.IntegrationTests.Services.TestEntities
{
    public class GetTestEntityByIdQuery
    {
        public Guid Id { get; set; }

        public static GetTestEntityByIdQuery Create(Guid id)
        {
            return new GetTestEntityByIdQuery
            {
                Id = id
            };
        }
    }
}