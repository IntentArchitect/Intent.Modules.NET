using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class FindCustomerByNameTests : BaseIntegrationTest
    {
        public FindCustomerByNameTests(IntegrationTestWebAppFactory factory) : base(factory)
        {

        }

        /// <summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        /// Filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Category!=ExcludeOnCI
        /// </summary>
        [Fact]
        [Trait("Requirement", "CosmosDB")]
        public async Task GFindCustomerByName_ShouldGetCustomerByNamed()
        {
            //Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var customerId = await dataFactory.CreateCustomer();
            var customer = await client.GetCustomerByIdAsync(customerId);

            //Act
            var foundCustomer = await client.FindCustomerByNameAsync(customer.Name);

            //Assert
            Assert.NotNull(foundCustomer);
            Assert.Equal(customer.Name, foundCustomer.Name);
        }
    }
}