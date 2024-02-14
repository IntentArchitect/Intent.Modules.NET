using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.Customers;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateCustomerTests : BaseIntegrationTest
    {
        public CreateCustomerTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// The Cosmos DB Linux Emulator Docker image does not run on Microsoft's CI environment (GitHub, Azure DevOps).")] // https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/45.
        /// Filter this test out of your CI/CD if appropriate e.g. dotnet test --filter Category!=ExcludeOnCI
        /// </summary>
        [Fact]
        [Trait("Category", "ExcludeOnCI")]
        public async Task CreateCustomer_ShouldCreateCustomer()
        {
            //Arrange
            var client = new CustomersHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateCustomerCommand>();

            //Act
            var customerId = await client.CreateCustomerAsync(command);

            //Assert
            var customer = await client.GetCustomerByIdAsync(customerId);
            Assert.NotNull(customer);
        }
    }
}