using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Orders;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateOrderOrderItemTests : BaseIntegrationTest
    {
        public CreateOrderOrderItemTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        /// Filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Category!=ExcludeOnCI
        /// </summary>
        [Fact]
        [Trait(Category, ExcludeOnCI)]
        public async Task CreateOrderOrderItem_ShouldCreateOrderOrderItem()
        {
            //Arrange
            var client = new OrdersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var orderId = await dataFactory.CreateOrderItemDependencies();

            var command = dataFactory.CreateCommand<CreateOrderOrderItemCommand>();

            //Act
            var orderItemId = await client.CreateOrderOrderItemAsync(command);

            //Assert
            var orderItem = await client.GetOrderOrderItemByIdAsync(orderId, orderItemId);
            Assert.NotNull(orderItem);
        }
    }
}