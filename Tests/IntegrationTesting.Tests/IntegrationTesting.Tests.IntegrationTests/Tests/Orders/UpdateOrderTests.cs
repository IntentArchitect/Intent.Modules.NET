using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.Orders;
using IntegrationTesting.Tests.IntegrationTests.Services.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateOrderTests : BaseIntegrationTest
    {
        public UpdateOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateOrder_ShouldUpdateOrder()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrder();

            var command = dataFactory.CreateCommand<UpdateOrderCommand>();
            command.Id = orderId;

            // Act
            await client.UpdateOrderAsync(orderId, command);

            // Assert
            var order = await client.GetOrderByIdAsync(orderId);
            Assert.NotNull(order);
            Assert.Equal(command.RefNo, order.RefNo);
        }
    }
}