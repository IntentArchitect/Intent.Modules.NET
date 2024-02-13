using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DiffIds
{
    public class GetDiffIdByIdQuery
    {
        public Guid Id { get; set; }

        public static GetDiffIdByIdQuery Create(Guid id)
        {
            return new GetDiffIdByIdQuery
            {
                Id = id
            };
        }
    }
}