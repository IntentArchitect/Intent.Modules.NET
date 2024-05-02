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
    public class UpdateEntityAppDefaultTests : BaseIntegrationTest
    {
        public UpdateEntityAppDefaultTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateEntityAppDefault_ShouldUpdateEntityAppDefault()
        {
            //Arrange
            var client = new EntityAppDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var entityAppDefaultId = await dataFactory.CreateEntityAppDefault();

            var command = dataFactory.CreateCommand<UpdateEntityAppDefaultCommand>();
            command.Id = entityAppDefaultId;

            //Act
            await client.UpdateEntityAppDefaultAsync(entityAppDefaultId, command);

            //Assert
            var entityAppDefault = await client.GetEntityAppDefaultByIdAsync(entityAppDefaultId);
            Assert.NotNull(entityAppDefault);
            Assert.Equal(command.Message, entityAppDefault.Message);
        }
    }
}