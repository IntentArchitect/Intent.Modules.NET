using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.CheckNewCompChildCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetCheckNewCompChildCrudsTests : BaseIntegrationTest
    {
        public GetCheckNewCompChildCrudsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCheckNewCompChildCruds_ShouldGetCheckNewCompChildCruds()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateCheckNewCompChildCrud();

            // Act
            var checkNewCompChildCruds = await client.GetCheckNewCompChildCrudsAsync();

            // Assert
            Assert.NotEmpty(checkNewCompChildCruds);
        }
    }
}