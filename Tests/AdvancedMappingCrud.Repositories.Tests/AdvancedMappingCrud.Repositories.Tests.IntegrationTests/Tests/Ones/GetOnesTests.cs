using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Ones;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.Ones
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetOnesTests : BaseIntegrationTest
    {
        public GetOnesTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetOnes_ShouldGetOnes()
        {
            // Arrange
            var client = new OnesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateOne();

            // Act
            var ones = await client.GetOnesAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.NotEmpty(ones);
        }
    }
}