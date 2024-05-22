using AutoFixture;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.IntegrationTests.HttpClients.TestEntities;
using ValueObjects.Class.IntegrationTests.Services.TestEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace ValueObjects.Class.IntegrationTests
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

        public async Task<Guid> CreateTestEntity()
        {
            var client = new TestEntitiesHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateTestEntityCommand>();
            var testEntityId = await client.CreateTestEntityAsync(command);
            _idTracker["TestEntityId"] = testEntityId;
            return testEntityId;
        }
    }
}