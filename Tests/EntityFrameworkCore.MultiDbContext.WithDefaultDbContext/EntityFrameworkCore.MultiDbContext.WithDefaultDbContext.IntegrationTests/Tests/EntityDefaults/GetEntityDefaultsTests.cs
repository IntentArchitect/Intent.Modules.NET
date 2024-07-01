using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetEntityDefaultsTests : BaseIntegrationTest
    {
        public GetEntityDefaultsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetEntityDefaults_ShouldGetEntityDefaults()
        {
            // Arrange
            var client = new EntityDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateEntityDefault();

            // Act
            var entityDefaults = await client.GetEntityDefaultsAsync();

            // Assert
            Assert.True(entityDefaults.Count > 0);
        }
    }
}