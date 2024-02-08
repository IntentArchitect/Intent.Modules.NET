using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.NoReturns
{
    public class GetNoReturnByIdQuery
    {
        public Guid Id { get; set; }

        public static GetNoReturnByIdQuery Create(Guid id)
        {
            return new GetNoReturnByIdQuery
            {
                Id = id
            };
        }
    }
}