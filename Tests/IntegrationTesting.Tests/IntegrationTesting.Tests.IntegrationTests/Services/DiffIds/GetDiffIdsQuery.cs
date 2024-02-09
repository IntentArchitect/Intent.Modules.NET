using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.DiffIds
{
    public class GetDiffIdsQuery
    {
        public static GetDiffIdsQuery Create()
        {
            return new GetDiffIdsQuery
            {
            };
        }
    }
}