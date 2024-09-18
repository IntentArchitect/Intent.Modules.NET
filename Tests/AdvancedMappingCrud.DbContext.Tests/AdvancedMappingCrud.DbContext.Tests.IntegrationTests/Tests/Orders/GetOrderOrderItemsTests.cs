using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetOrderOrderItemsTests : BaseIntegrationTest
    {
        public GetOrderOrderItemsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetOrderOrderItems_ShouldGetOrderOrderItems()
        {
            // Arrange
            var integrationClient = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateOrderItem();

            // Act
            var orderItems = await integrationClient.GetOrderOrderItemsAsync(ids.OrderId);

            // Assert
            Assert.True(orderItems.Count > 0);
        }
    }
}