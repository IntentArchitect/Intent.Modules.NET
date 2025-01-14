using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.CheckNewCompChildCruds
{
    public class GetCheckNewCompChildCrudsQuery
    {
        public static GetCheckNewCompChildCrudsQuery Create()
        {
            return new GetCheckNewCompChildCrudsQuery
            {
            };
        }
    }
}