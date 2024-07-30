using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.HasMissingDeps;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateHasMissingDepTests : BaseIntegrationTest
    {
        public CreateHasMissingDepTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [IntentManaged(Mode.Fully, Signature = Mode.Ignore, Attributes = Mode.Ignore)]
        public async Task CreateHasMissingDep_ShouldCreateHasMissingDep()
        {
            // Arrange
            var client = new HasMissingDepsHttpClient(CreateClient());

            // Act

            // Unable to generate test: Can't determine how to mock data for (MissingDep)
            // TODO: Implement CreateHasMissingDep_ShouldCreateHasMissingDep (CreateHasMissingDepTests) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}