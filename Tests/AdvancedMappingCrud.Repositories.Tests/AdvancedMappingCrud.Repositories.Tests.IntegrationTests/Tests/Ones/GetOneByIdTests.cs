using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Ones;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.Ones
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetOneByIdTests : BaseIntegrationTest
    {
        public GetOneByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetOneById_ShouldGetOneById()
        {
            // Arrange
            var client = new OnesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var oneId = await dataFactory.CreateOne();

            // Act
            var one = await client.GetOneByIdAsync(oneId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(one);
        }
    }
}