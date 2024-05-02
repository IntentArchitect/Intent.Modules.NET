using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAppDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetEntityAppDefaultByIdTests : BaseIntegrationTest
    {
        public GetEntityAppDefaultByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetEntityAppDefaultById_ShouldGetEntityAppDefaultById()
        {
            //Arrange
            var client = new EntityAppDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var entityAppDefaultId = await dataFactory.CreateEntityAppDefault();

            //Act
            var entityAppDefault = await client.GetEntityAppDefaultByIdAsync(entityAppDefaultId);

            //Assert
            Assert.NotNull(entityAppDefault);
        }
    }
}