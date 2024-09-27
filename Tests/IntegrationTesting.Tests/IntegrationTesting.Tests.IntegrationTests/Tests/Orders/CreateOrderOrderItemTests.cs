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
    public class CreateOrderOrderItemTests : BaseIntegrationTest
    {
        public CreateOrderOrderItemTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateOrderOrderItem_ShouldCreateOrderOrderItem()
        {
            // Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrderItemDependencies();

            var command = dataFactory.CreateCommand<CreateOrderOrderItemCommand>();

            // Act
            var orderItemId = await client.CreateOrderOrderItemAsync(command);

            // Assert
            var orderItem = await client.GetOrderOrderItemByIdAsync(orderId, orderItemId);
            Assert.NotNull(orderItem);
        }
    }
}