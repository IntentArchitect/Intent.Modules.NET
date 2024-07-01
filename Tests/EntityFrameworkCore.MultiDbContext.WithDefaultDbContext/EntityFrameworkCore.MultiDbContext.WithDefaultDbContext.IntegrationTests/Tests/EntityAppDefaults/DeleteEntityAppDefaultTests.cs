using System.Net;
using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAppDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteEntityAppDefaultTests : BaseIntegrationTest
    {
        public DeleteEntityAppDefaultTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteEntityAppDefault_ShouldDeleteEntityAppDefault()
        {
            // Arrange
            var client = new EntityAppDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var entityAppDefaultId = await dataFactory.CreateEntityAppDefault();

            // Act
            await client.DeleteEntityAppDefaultAsync(entityAppDefaultId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetEntityAppDefaultByIdAsync(entityAppDefaultId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}