using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.OperationMapping;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.OperationMapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetTaskItemByIdTests : BaseIntegrationTest
    {
        public GetTaskItemByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetTaskItemById_ShouldGetTaskItemById()
        {
            // Arrange
            var client = new OperationMappingHttpClient(CreateClient());

            // Act

            // Unable to generate test: Can't determine how to mock data for (TaskList)
            // TODO: Implement GetTaskItemById_ShouldGetTaskItemById (GetTaskItemByIdTests) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}