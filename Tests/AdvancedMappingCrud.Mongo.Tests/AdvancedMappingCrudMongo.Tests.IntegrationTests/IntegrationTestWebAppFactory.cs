using AdvancedMappingCrudMongo.Tests.Api;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.IntegrationTestWebAppFactory", Version = "1.0")]

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public IntegrationTestWebAppFactory()
        {
            MongoDbFixture = new MongoDbContainerFixture();
        }

        public MongoDbContainerFixture MongoDbFixture { get; }

        public async Task InitializeAsync()
        {
            await MongoDbFixture.InitializeAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await MongoDbFixture.DisposeAsync();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var result = base.CreateHost(builder);
            MongoDbFixture.OnHostCreation(result.Services);
            return result;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                MongoDbFixture.ConfigureTestServices(services);
            });
        }
    }
}