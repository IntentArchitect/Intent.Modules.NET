using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.UserService
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateUserWithTaskItemServiceTests : BaseIntegrationTest
    {
        public CreateUserWithTaskItemServiceTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}