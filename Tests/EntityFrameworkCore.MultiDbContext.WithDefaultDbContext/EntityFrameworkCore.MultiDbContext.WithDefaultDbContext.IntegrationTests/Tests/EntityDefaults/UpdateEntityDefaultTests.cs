using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateEntityDefaultTests : BaseIntegrationTest
    {
        public UpdateEntityDefaultTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateEntityDefault_ShouldUpdateEntityDefault()
        {
            // Arrange
            var client = new EntityDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var entityDefaultId = await dataFactory.CreateEntityDefault();

            var command = dataFactory.CreateCommand<UpdateEntityDefaultCommand>();
            command.Id = entityDefaultId;

            // Act
            await client.UpdateEntityDefaultAsync(entityDefaultId, command);

            // Assert
            var entityDefault = await client.GetEntityDefaultByIdAsync(entityDefaultId);
            Assert.NotNull(entityDefault);
            Assert.Equal(command.Message, entityDefault.Message);
        }
    }
}