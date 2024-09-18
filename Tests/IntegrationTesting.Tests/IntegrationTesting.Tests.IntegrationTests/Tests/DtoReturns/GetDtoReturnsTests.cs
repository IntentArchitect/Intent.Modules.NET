using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DtoReturns;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetDtoReturnsTests : BaseIntegrationTest
    {
        public GetDtoReturnsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDtoReturns_ShouldGetDtoReturns()
        {
            // Arrange
            var integrationClient = new DtoReturnsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateDtoReturn();

            // Act
            var dtoReturns = await integrationClient.GetDtoReturnsAsync();

            // Assert
            Assert.True(dtoReturns.Count > 0);
        }
    }
}