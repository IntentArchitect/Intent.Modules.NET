using AutoFixture;
using EfCoreSoftDelete.IntegrationTests.HttpClients.Customers;
using EfCoreSoftDelete.IntegrationTests.Services.Customers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace EfCoreSoftDelete.IntegrationTests
{
    public class TestDataFactory
    {
        private readonly Fixture _fixture;
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly Dictionary<string, object> _idTracker = new();

        public TestDataFactory(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
            _fixture = new Fixture();
        }

        public T CreateCommand<T>()
        {
            var fixture = new Fixture();
            fixture.RepeatCount = 1;
            fixture.Customizations.Add(new PopulateIdsSpecimenBuilder(_idTracker));
            return fixture.Create<T>();
        }

        public async Task<Guid> CreateCustomer()
        {
            var client = new CustomersHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateCustomerCommand>();
            var customerId = await client.CreateCustomerAsync(command);
            _idTracker["CustomerId"] = customerId;
            return customerId;
        }
    }
}