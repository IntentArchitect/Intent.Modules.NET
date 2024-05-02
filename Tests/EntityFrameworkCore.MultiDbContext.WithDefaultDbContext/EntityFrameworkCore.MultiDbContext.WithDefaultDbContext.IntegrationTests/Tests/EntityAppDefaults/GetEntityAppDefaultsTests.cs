using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAppDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetEntityAppDefaultsTests : BaseIntegrationTest
    {
        public GetEntityAppDefaultsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetEntityAppDefaults_ShouldGetEntityAppDefaults()
        {
            //Arrange
            var client = new EntityAppDefaultsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateEntityAppDefault();

            //Act
            var entityAppDefaults = await client.GetEntityAppDefaultsAsync();

            //Assert
            Assert.True(entityAppDefaults.Count > 0);
        }
    }
}