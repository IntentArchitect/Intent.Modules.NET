using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns
{
    public class GetDtoReturnByIdQuery
    {
        public Guid Id { get; set; }

        public static GetDtoReturnByIdQuery Create(Guid id)
        {
            return new GetDtoReturnByIdQuery
            {
                Id = id
            };
        }
    }
}