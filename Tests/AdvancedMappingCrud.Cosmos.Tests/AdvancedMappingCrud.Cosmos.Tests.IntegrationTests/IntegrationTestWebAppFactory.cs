using AdvancedMappingCrud.Cosmos.Tests.Api;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.IntegrationTestWebAppFactory", Version = "1.0")]

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public IntegrationTestWebAppFactory()
        {
            CosmosContainerFixture = new CosmosContainerFixture();
        }

        public CosmosContainerFixture CosmosContainerFixture { get; }

        public async ValueTask InitializeAsync()
        {
            await CosmosContainerFixture.InitializeAsync();
        }

        public override async ValueTask DisposeAsync()
        {
            await CosmosContainerFixture.DisposeAsync();
            await base.DisposeAsync();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var result = base.CreateHost(builder);
            CosmosContainerFixture.OnHostCreation(result.Services);
            return result;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                CosmosContainerFixture.ConfigureTestServices(services);
            });
        }
    }
}