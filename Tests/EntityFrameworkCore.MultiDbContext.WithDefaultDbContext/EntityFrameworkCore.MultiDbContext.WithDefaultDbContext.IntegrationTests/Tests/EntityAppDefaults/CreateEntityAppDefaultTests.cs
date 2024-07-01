using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAppDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAppDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateEntityAppDefaultTests : BaseIntegrationTest
    {
        public CreateEntityAppDefaultTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateEntityAppDefault_ShouldCreateEntityAppDefault()
        {
            // Arrange
            var client = new EntityAppDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateEntityAppDefaultCommand>();

            // Act
            var entityAppDefaultId = await client.CreateEntityAppDefaultAsync(command);

            // Assert
            var entityAppDefault = await client.GetEntityAppDefaultByIdAsync(entityAppDefaultId);
            Assert.NotNull(entityAppDefault);
        }
    }
}