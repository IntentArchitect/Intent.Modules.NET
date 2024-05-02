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
    public class CreateEntityDefaultTests : BaseIntegrationTest
    {
        public CreateEntityDefaultTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateEntityDefault_ShouldCreateEntityDefault()
        {
            //Arrange
            var client = new EntityDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateEntityDefaultCommand>();

            //Act
            var entityDefaultId = await client.CreateEntityDefaultAsync(command);

            //Assert
            var entityDefault = await client.GetEntityDefaultByIdAsync(entityDefaultId);
            Assert.NotNull(entityDefault);
        }
    }
}