using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateEntityAfterEtagWasChangedByPreviousOperationTestTests : BaseIntegrationTest
    {
        public UpdateEntityAfterEtagWasChangedByPreviousOperationTestTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}