using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.OperationMapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateUserWithTaskItemTests : BaseIntegrationTest
    {
        public CreateUserWithTaskItemTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}