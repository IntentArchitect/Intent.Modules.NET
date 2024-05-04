using AutoFixture;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAlternates;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAppDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAlternates;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAppDefaults;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.TestDataFactory", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests
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

        public async Task<Guid> CreateEntityAlternate()
        {
            var client = new EntityAlternatesHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateEntityAlternateCommand>();
            var entityAlternateId = await client.CreateEntityAlternateAsync(command);
            _idTracker["EntityAlternateId"] = entityAlternateId;
            return entityAlternateId;
        }

        public async Task<Guid> CreateEntityAppDefault()
        {
            var client = new EntityAppDefaultsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateEntityAppDefaultCommand>();
            var entityAppDefaultId = await client.CreateEntityAppDefaultAsync(command);
            _idTracker["EntityAppDefaultId"] = entityAppDefaultId;
            return entityAppDefaultId;
        }

        public async Task<Guid> CreateEntityDefault()
        {
            var client = new EntityDefaultsHttpClient(_factory.CreateClient());

            var command = CreateCommand<CreateEntityDefaultCommand>();
            var entityDefaultId = await client.CreateEntityDefaultAsync(command);
            _idTracker["EntityDefaultId"] = entityDefaultId;
            return entityDefaultId;
        }
    }
}