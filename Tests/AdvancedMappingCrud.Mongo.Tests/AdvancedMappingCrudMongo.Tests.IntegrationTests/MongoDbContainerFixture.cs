using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.MongoDbContainerFixture", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests
{
    public class MongoDbContainerFixture
    {
        private const string DatabaseName = "IntegrationTestDb";
        private readonly MongoDbContainer _dbContainer;

        public MongoDbContainerFixture()
        {
            _dbContainer = new MongoDbBuilder()
                                      .WithImage("mongo:latest")
                                      .Build();
        }

        public void ConfigureTestServices(IServiceCollection services)
        {
            string connectionString = GetTestConnectionString(_dbContainer.GetConnectionString());
            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
            services.AddSingleton(sp =>
                                        {
                                            var client = sp.GetRequiredService<IMongoClient>();
                                            return client.GetDatabase(DatabaseName);
                                        });
        }

        public void OnHostCreation(IServiceProvider services)
        {
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }

        private string GetTestConnectionString(string containerConnectionString)
        {
            if (!containerConnectionString.Contains("?"))
            {
                return containerConnectionString + "IntegrationTestDb?";
            }
            else
            {
                return containerConnectionString.Replace("?", "IntegrationTestDb?") + "&authSource=admin";
            }
        }
    }
}