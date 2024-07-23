using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Basics;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetBasicsTests : BaseIntegrationTest
    {
        public GetBasicsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBasics_ShouldGetBasics()
        {
            // Arrange
            var client = new BasicsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateBasic();

            // Act
            var customers = await client.GetBasicsAsync(1, 10, "Name asc");

            // Assert
            Assert.True(customers.Data.Count() > 0);
        }
    }
}