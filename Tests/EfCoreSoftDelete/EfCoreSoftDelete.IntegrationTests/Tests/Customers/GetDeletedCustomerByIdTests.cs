using EfCoreSoftDelete.IntegrationTests.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EfCoreSoftDelete.IntegrationTests.Tests.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetDeletedCustomerByIdTests : BaseIntegrationTest
    {
        public GetDeletedCustomerByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {

        }

        [Fact]
        public async Task ItShouldNotDeleteOwnedEntities()
        {
            // Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();

            // Act
            await client.DeleteCustomerAsync(customerId, TestContext.Current.CancellationToken);

            // Assert
            var customer = await client.GetDeletedCustomerByIdAsync(customerId, TestContext.Current.CancellationToken);
            Assert.NotNull(customer);
            Assert.True(customer.IsDeleted);
            Assert.NotNull(customer.PrimaryAddress?.PrimaryBuilding);
            Assert.NotNull(customer.PrimaryAddress?.OtherBuildings?.SingleOrDefault());
            Assert.NotNull(customer.OtherAddresses?.SingleOrDefault()?.PrimaryBuilding);
            Assert.NotNull(customer.OtherAddresses?.SingleOrDefault()?.OtherBuildings?.SingleOrDefault());
        }
    }
}