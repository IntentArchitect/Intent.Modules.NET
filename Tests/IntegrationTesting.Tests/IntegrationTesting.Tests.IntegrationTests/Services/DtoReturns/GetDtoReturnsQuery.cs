using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns
{
    public class GetDtoReturnsQuery
    {
        public static GetDtoReturnsQuery Create()
        {
            return new GetDtoReturnsQuery
            {
            };
        }
    }
}