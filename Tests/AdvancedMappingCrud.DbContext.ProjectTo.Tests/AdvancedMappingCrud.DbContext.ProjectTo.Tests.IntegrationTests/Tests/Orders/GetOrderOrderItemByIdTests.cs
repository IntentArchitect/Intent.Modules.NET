using AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.HttpClients.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests.Orders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetOrderOrderItemByIdTests : BaseIntegrationTest
    {
        public GetOrderOrderItemByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetOrderOrderItemById_ShouldGetOrderOrderItemById()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateOrderItem();

            // Act
            var orderItem = await client.GetOrderOrderItemByIdAsync(ids.OrderId, ids.OrderItemId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(orderItem);
        }
    }
}