using System.Net;
using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteEntityDefaultTests : BaseIntegrationTest
    {
        public DeleteEntityDefaultTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteEntityDefault_ShouldDeleteEntityDefault()
        {
            //Arrange
            var client = new EntityDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var entityDefaultId = await dataFactory.CreateEntityDefault();

            //Act
            await client.DeleteEntityDefaultAsync(entityDefaultId);

            //Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetEntityDefaultByIdAsync(entityDefaultId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}