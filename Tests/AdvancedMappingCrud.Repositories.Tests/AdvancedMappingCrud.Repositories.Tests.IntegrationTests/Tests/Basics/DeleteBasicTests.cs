using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteBasicTests : BaseIntegrationTest
    {
        public DeleteBasicTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteBasic_ShouldDeleteBasic()
        {
            // Arrange
            var integrationClient = new BasicsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var basicId = await dataFactory.CreateBasic();

            // Act
            await integrationClient.DeleteBasicAsync(basicId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => integrationClient.GetBasicByIdAsync(basicId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}