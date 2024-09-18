using AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetOrderByIdTests : BaseIntegrationTest
    {
        public GetOrderByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetOrderById_ShouldGetOrderById()
        {
            // Arrange
            var integrationClient = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrder();

            // Act
            var order = await integrationClient.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(order);
        }
    }
}