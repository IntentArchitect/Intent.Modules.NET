using Ardalis.IntegrationTests.HttpClients.Clients;
using Ardalis.IntegrationTests.Services.Clients;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace Ardalis.IntegrationTests
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

        public async Task<Guid> CreateClient()
        {
            var client = new ClientsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateClientCommand>();
            var clientId = await client.CreateClientAsync(command);
            _idTracker["ClientId"] = clientId;
            return clientId;
        }
    }
}