using System.Net;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteOrderOrderItemTests : BaseIntegrationTest
    {
        public DeleteOrderOrderItemTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact(Skip = "The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        public async Task DeleteOrderOrderItem_ShouldDeleteOrderOrderItem()
        {
            //Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var ids = await dataFactory.CreateOrderItem();

            //Act
            await client.DeleteOrderOrderItemAsync(ids.OrderId, ids.OrderItemId);

            //Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetOrderOrderItemByIdAsync(ids.OrderId, ids.OrderItemId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}