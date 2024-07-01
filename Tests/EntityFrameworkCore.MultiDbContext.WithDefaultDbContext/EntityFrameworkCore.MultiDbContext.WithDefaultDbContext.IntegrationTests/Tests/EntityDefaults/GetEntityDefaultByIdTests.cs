using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetEntityDefaultByIdTests : BaseIntegrationTest
    {
        public GetEntityDefaultByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetEntityDefaultById_ShouldGetEntityDefaultById()
        {
            // Arrange
            var client = new EntityDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var entityDefaultId = await dataFactory.CreateEntityDefault();

            // Act
            var entityDefault = await client.GetEntityDefaultByIdAsync(entityDefaultId);

            // Assert
            Assert.NotNull(entityDefault);
        }
    }
}